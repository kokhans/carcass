// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text;
using Carcass.Core;
using Carcass.Data.Core.Aggregates.ResolutionStrategies.Abstracts;
using Carcass.Data.Core.Checkpoints.Abstracts;
using Carcass.Data.Core.Checkpoints.Repositories.Abstracts;
using Carcass.Data.Core.DomainEvents.Abstracts;
using Carcass.Data.Core.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.EventStoreDb.Aggregates.ResolutionStrategies.Extensions;
using Carcass.Data.EventStoreDb.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using Carcass.Logging.Core.Adapters;
using Carcass.Logging.Core.Adapters.Abstracts;
using EventStore.Client;
using Microsoft.Extensions.Hosting;

// ReSharper disable VirtualMemberCallInConstructor

namespace Carcass.Data.EventStoreDb.HostedServices.Abstracts;

public abstract class EventStoreDbSubscriptionHostedService : IHostedService
{
    private readonly LoggerAdapter<EventStoreDbSubscriptionHostedService> _loggerAdapter;
    private readonly IJsonProvider _jsonProvider;
    private readonly IDomainEventLocator _domainEventLocator;
    private readonly IDomainEventUpgraderDispatcher _domainEventUpgraderDispatcher;
    private readonly ICheckpointRepository _checkpointRepository;
    private readonly EventStoreClient _eventStoreClient;
    private readonly EventStorePersistentSubscriptionsClient _persistentSubscriptionsClient;
    private readonly string _streamName;
    private PersistentSubscription? _persistentSubscription;

    protected EventStoreDbSubscriptionHostedService(
        ILoggerAdapterFactory loggerAdapterFactory,
        IJsonProvider jsonProvider,
        IAggregateNameResolutionStrategy aggregateNameResolutionStrategy,
        IDomainEventLocator domainEventLocator,
        IDomainEventUpgraderDispatcher domainEventUpgraderDispatcher,
        ICheckpointRepository checkpointRepository,
        EventStoreClient eventStoreClient,
        EventStorePersistentSubscriptionsClient persistentSubscriptionsClient
    )
    {
        ArgumentVerifier.NotNull(loggerAdapterFactory, nameof(loggerAdapterFactory));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));
        ArgumentVerifier.NotNull(aggregateNameResolutionStrategy, nameof(aggregateNameResolutionStrategy));
        ArgumentVerifier.NotNull(domainEventLocator, nameof(domainEventLocator));
        ArgumentVerifier.NotNull(domainEventUpgraderDispatcher, nameof(domainEventUpgraderDispatcher));
        ArgumentVerifier.NotNull(checkpointRepository, nameof(checkpointRepository));
        ArgumentVerifier.NotNull(eventStoreClient, nameof(eventStoreClient));
        ArgumentVerifier.NotNull(persistentSubscriptionsClient, nameof(persistentSubscriptionsClient));

        _loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<EventStoreDbSubscriptionHostedService>();
        _jsonProvider = jsonProvider;
        _domainEventLocator = domainEventLocator;
        _domainEventUpgraderDispatcher = domainEventUpgraderDispatcher;
        _checkpointRepository = checkpointRepository;
        _eventStoreClient = eventStoreClient;
        _persistentSubscriptionsClient = persistentSubscriptionsClient;
        _streamName = aggregateNameResolutionStrategy.GetEventStoreDbPersistentSubscriptionStreamName(AggregateName);
    }

    protected abstract string AggregateName { get; }
    protected abstract string GroupName { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await SubscribeAsync();
        }
        catch (Exception exception)
        {
            _loggerAdapter.LogError(exception);
            await SubscribeAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _persistentSubscription?.Dispose();

        return Task.CompletedTask;
    }

    protected abstract Task<bool> HandleDomainEventAsync(IDomainEvent domainEvent, ResolvedEvent resolvedEvent);

    private async Task SubscribeAsync()
    {
        _persistentSubscription = await _persistentSubscriptionsClient
            .SubscribeToStreamAsync(_streamName, GroupName, async (_, incomeResolvedEvent, _, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    ICheckpoint? lastCheckpoint =
                        await _checkpointRepository.LoadCheckpointAsync(_streamName, GroupName, cancellationToken);
                    if (
                        lastCheckpoint is not null &&
                        lastCheckpoint.CommittedPosition > incomeResolvedEvent.OriginalEventNumber.ToInt64()
                    ) return;

                    long nextPageStart = lastCheckpoint?.CommittedPosition + 1 ?? 0;
                    do
                    {
                        EventStoreClient.ReadStreamResult readStreamResult =
                            _eventStoreClient.ReadStreamAsync(
                                Direction.Forwards,
                                _streamName,
                                StreamPosition.FromInt64(nextPageStart),
                                4096,
                                resolveLinkTos: true,
                                cancellationToken: cancellationToken
                            );

                        List<ResolvedEvent> resolvedEvents = new();
                        try
                        {
                            resolvedEvents = await readStreamResult.ToListAsync(cancellationToken);
                        }
                        catch (StreamNotFoundException)
                        {
                        }

                        if (resolvedEvents.Any())
                        {
                            foreach (ResolvedEvent resolvedEvent in resolvedEvents)
                            {
                                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                                if (resolvedEvent.Event is null)
                                {
                                    _loggerAdapter.LogError(BuildLogMessage(
                                            resolvedEvent,
                                            "Skip",
                                            $"Event in position {resolvedEvent.OriginalEventNumber} is null."
                                        )
                                    );
                                    continue;
                                }

                                if (resolvedEvent.Event.EventStreamId.StartsWith("$$") ||
                                    resolvedEvent.Event.EventType == "$metadata")
                                {
                                    _loggerAdapter.LogError(BuildLogMessage(
                                            resolvedEvent,
                                            "Skip",
                                            $"Event {resolvedEvent.Event.EventId} is deleted."
                                        )
                                    );
                                    continue;
                                }

                                _loggerAdapter.LogInformation(BuildLogMessage(resolvedEvent, "Accept"));

                                if (!resolvedEvent.TryGetDomainEvent(
                                        out IDomainEvent? domainEvent,
                                        _domainEventLocator,
                                        _jsonProvider
                                    )
                                   ) continue;

                                IDomainEvent upgradedDomainEvent =
                                    _domainEventUpgraderDispatcher.DispatchDomainEvent(domainEvent!);

                                if (await HandleDomainEventAsync(upgradedDomainEvent, resolvedEvent)) continue;

                                await _checkpointRepository.SaveCheckpointAsync(
                                    _streamName,
                                    GroupName,
                                    resolvedEvent.OriginalEventNumber.ToInt64(),
                                    cancellationToken
                                );

                                _loggerAdapter.LogInformation(BuildLogMessage(resolvedEvent, "Ack"));
                            }

                            nextPageStart = await readStreamResult.GetAsyncEnumerator(cancellationToken).MoveNextAsync()
                                ? nextPageStart + 1
                                : -1;
                        }
                        else
                        {
                            nextPageStart = -1;
                        }
                    } while (nextPageStart != -1);
                }, (_, subscriptionDroppedReason, exception) =>
                {
                    _loggerAdapter.LogError(subscriptionDroppedReason.ToString(), exception);
                    SubscribeAsync().GetAwaiter().GetResult();
                }
            );
    }

    protected string BuildLogMessage(ResolvedEvent resolvedEvent, string operationName, string? reason = default)
    {
        ArgumentVerifier.NotNull(operationName, nameof(operationName));

        StringBuilder stringBuilder = new();
        stringBuilder.Append($"Update {AggregateName} ({operationName}) - ");
        stringBuilder.Append($"StreamId: {resolvedEvent.OriginalStreamId} | ");
        stringBuilder.Append($"EventId: {resolvedEvent.Event.EventId} | ");
        stringBuilder.Append($"EventNumber: {resolvedEvent.OriginalEventNumber}");

        if (reason is not null)
            stringBuilder.Append($"Reason: {reason}");

        return stringBuilder.ToString();
    }

    protected void LogEventTypeHandlerNotFound(ResolvedEvent resolvedEvent) =>
        _loggerAdapter.LogError(BuildLogMessage(
                resolvedEvent,
                "Skip",
                $"Event {resolvedEvent.Event.EventId} type handler {resolvedEvent.Event.EventType} not found."
            )
        );
}