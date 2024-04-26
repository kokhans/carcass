// MIT License
//
// Copyright (c) 2022-2025 Serhii Kokhan
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
using Carcass.Data.Core.EventSourcing.Aggregates.ResolutionStrategies.Abstracts;
using Carcass.Data.Core.EventSourcing.Checkpoints.Abstracts;
using Carcass.Data.Core.EventSourcing.Checkpoints.Repositories.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.EventStoreDb.Aggregates.ResolutionStrategies.Extensions;
using Carcass.Data.EventStoreDb.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using Carcass.Logging.Adapters;
using Carcass.Logging.Adapters.Abstracts;
using EventStore.Client;
using Microsoft.Extensions.Hosting;

// ReSharper disable VirtualMemberCallInConstructor

namespace Carcass.Data.EventStoreDb.HostedServices.Abstracts;

// ReSharper disable once UnusedType.Global
/// <summary>
///     A hosted service that manages subscriptions to EventStoreDB streams, processes domain events,
///     and handles event upgrades and checkpoints. This class serves as an abstraction for creating
///     specific subscriptions tailored to particular aggregate types.
/// </summary>
/// <remarks>
///     Implementors of this class must provide concrete logic for handling domain events,
///     as well as specifying the aggregate and group names for the subscription.
/// </remarks>
public abstract class EventStoreDbSubscriptionHostedService : IHostedService
{
    /// <summary>
    ///     Represents the repository used to manage checkpoints for event-sourcing subscriptions.
    ///     It is responsible for storing and retrieving the last successfully processed position of an event stream.
    ///     This enables resumption of event processing from the correct position in case of interruptions.
    /// </summary>
    /// <remarks>
    ///     Checkpoints ensure that event subscriptions can consistently and reliably process events without duplications or
    ///     omissions.
    /// </remarks>
    private readonly ICheckpointRepository _checkpointRepository;

    /// <summary>
    ///     Represents a service responsible for locating specific domain events within the application.
    ///     Used to resolve and deserialize domain events into their corresponding types during event handling.
    /// </summary>
    /// <remarks>
    ///     This member is part of the <see cref="EventStoreDbSubscriptionHostedService" /> and aids in
    ///     identifying and working with domain events during subscription and event processing.
    /// </remarks>
    private readonly IDomainEventLocator _domainEventLocator;

    /// <summary>
    ///     Responsible for dispatching and applying appropriate upgrades to domain events.
    /// </summary>
    /// <remarks>
    ///     Ensures domain events are transformed to their most recent versions during processing.
    /// </remarks>
    /// <exception cref="Exception">Thrown when a domain event upgrade fails during dispatch.</exception>
    private readonly IDomainEventUpgraderDispatcher _domainEventUpgraderDispatcher;

    /// <summary>
    ///     Represents a client instance for communicating with the Event Store database.
    ///     Used to read and interact with event streams in the database.
    /// </summary>
    private readonly EventStoreClient _eventStoreClient;

    /// <summary>
    ///     Provides an abstraction for handling JSON operations such as serialization and deserialization.
    /// </summary>
    private readonly IJsonProvider _jsonProvider;

    /// <summary>
    ///     An instance of <see cref="LoggerAdapter{TCategoryName}" /> used for logging events and errors
    ///     within the context of the <see cref="EventStoreDbSubscriptionHostedService" /> lifecycle.
    /// </summary>
    /// <remarks>
    ///     This variable facilitates structured and consistent logging behavior, encapsulating
    ///     the underlying logging mechanism provided by the application's dependency injection setup.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown during the service initialization if the logging adapter factory fails
    ///     to provide a valid instance of <see cref="LoggerAdapter{TCategoryName}" />.
    /// </exception>
    private readonly LoggerAdapter<EventStoreDbSubscriptionHostedService> _loggerAdapter;

    /// <summary>
    ///     Represents the EventStore persistent subscriptions client used to manage subscriptions
    ///     to persistent streams within the EventStore.
    /// </summary>
    private readonly EventStorePersistentSubscriptionsClient _persistentSubscriptionsClient;

    /// <summary>
    ///     Represents the name of the stream to which the hosted service will subscribe
    ///     for reading and processing events in the EventStoreDB.
    /// </summary>
    /// <remarks>
    ///     This variable holds the resolved stream name based on the aggregate name
    ///     resolution strategy provided to the service.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the aggregate name resolution strategy fails to provide a valid stream name.
    /// </exception>
    /// <returns>
    ///     A <c>string</c> representing the name of the EventStoreDB stream.
    /// </returns>
    private readonly string _streamName;

    /// <summary>
    ///     Represents the persistent subscription to an EventStore stream for this service.
    ///     This subscription manages the interaction with the EventStore Persistent Subscriptions client,
    ///     allowing events to be processed using the defined handler logic.
    /// </summary>
    /// <remarks>
    ///     This field is initialized when the subscription to the stream is created and disposed when the service stops.
    ///     The subscription ensures reliable delivery of events within the constraints of the EventStore Persistent
    ///     Subscription mechanism.
    /// </remarks>
    /// <exception cref="System.ObjectDisposedException">
    ///     Thrown if the persistent subscription is accessed after it has been disposed during service shutdown.
    /// </exception>
    private PersistentSubscription? _persistentSubscription;

    /// <summary>
    ///     Represents a hosted service for managing persistent subscriptions to EventStoreDB. This abstract class
    ///     provides a foundation for interacting with EventStoreDB, managing checkpoints, and handling domain events.
    /// </summary>
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

    /// <summary>
    ///     Gets the name of the aggregate associated with the EventStoreDb persistent subscription.
    ///     This property is used to resolve the stream name for handling specific domain events.
    /// </summary>
    /// <returns>
    ///     A string representing the name of the aggregate.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the aggregate name is not properly configured or resolved.
    /// </exception>
    protected abstract string AggregateName { get; }

    /// <summary>
    ///     Gets the name of the persistent subscription group associated with the EventStoreDB subscription.
    /// </summary>
    /// <value>
    ///     A string representing the group name for the persistent subscription in EventStoreDB.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the group name is not properly configured or is used incorrectly during subscription setup or handling.
    /// </exception>
    protected abstract string GroupName { get; }

    /// <summary>
    ///     Starts the hosted service, initializing the EventStoreDB subscription and handling any errors during the process.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous start operation.
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///     Thrown when the operation is canceled.
    /// </exception>
    /// <exception cref="System.Exception">
    ///     Thrown if an error occurs while establishing the subscription.
    /// </exception>
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

    /// <summary>
    ///     Stops the subscription service asynchronously, releasing any resources or connections associated with the
    ///     subscription.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token used to propagate notifications that the operation should be canceled.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous stop operation.
    /// </returns>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the subscription has already been disposed.
    /// </exception>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _persistentSubscription?.Dispose();

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Asynchronously handles a domain event and its associated resolved event.
    /// </summary>
    /// <param name="domainEvent">The domain event to be processed.</param>
    /// <param name="resolvedEvent">The resolved event containing metadata and event data.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. Returns <c>true</c> if the event was successfully processed
    ///     and should be acknowledged; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="domainEvent" /> or
    ///     <paramref name="resolvedEvent" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="Exception">Thrown when an error occurs during the event handling process.</exception>
    protected abstract Task<bool> HandleDomainEventAsync(IDomainEvent domainEvent, ResolvedEvent resolvedEvent);

    /// <summary>
    ///     Subscribes to a persistent event store stream with a specific group name and handles domain events.
    /// </summary>
    /// <returns>
    ///     A <see cref="Task" /> that represents the asynchronous operation of subscribing to the stream.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via the provided <see cref="CancellationToken" /> during subscription setup.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown if an unexpected error occurs while attempting to subscribe to the stream.
    /// </exception>
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
                                true,
                                cancellationToken: cancellationToken
                            );

                        List<ResolvedEvent> resolvedEvents = [];
                        try
                        {
                            resolvedEvents = await readStreamResult.ToListAsync(cancellationToken);
                        }
                        catch (StreamNotFoundException)
                        {
                        }

                        if (resolvedEvents.Count != 0)
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

                            await using IAsyncEnumerator<ResolvedEvent> asyncEnumerator =
                                readStreamResult.GetAsyncEnumerator(cancellationToken);
                            nextPageStart = await asyncEnumerator.MoveNextAsync()
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

    // ReSharper disable once MemberCanBePrivate.Global
    /// <summary>
    ///     Constructs a log message with details about the specified event, operation, and optional reason.
    /// </summary>
    /// <param name="resolvedEvent">
    ///     The resolved event containing relevant details such as stream ID, event ID, and event number.
    /// </param>
    /// <param name="operationName">
    ///     The name of the operation being performed (e.g., "Accept", "Skip").
    /// </param>
    /// <param name="reason">
    ///     An optional parameter providing a detailed explanation or reason related to the operation.
    /// </param>
    /// <returns>
    ///     A formatted string that includes information about the aggregate, stream ID, event ID, event number,
    ///     and an optional reason for the operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="operationName" /> is null.
    /// </exception>
    protected string BuildLogMessage(ResolvedEvent resolvedEvent, string operationName, string? reason = null)
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

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Logs an error message indicating that a handler for the specified event type was not found.
    /// </summary>
    /// <param name="resolvedEvent">The event for which the handler was not found.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="resolvedEvent" /> is null.</exception>
    protected void LogEventTypeHandlerNotFound(ResolvedEvent resolvedEvent)
    {
        _loggerAdapter.LogError(BuildLogMessage(
                resolvedEvent,
                "Skip",
                $"Event {resolvedEvent.Event.EventId} type handler {resolvedEvent.Event.EventType} not found."
            )
        );
    }
}