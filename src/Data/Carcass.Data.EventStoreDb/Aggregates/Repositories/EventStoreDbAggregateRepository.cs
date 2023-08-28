﻿// MIT License
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
using Carcass.Data.Core.EventSourcing.Aggregates.Abstracts;
using Carcass.Data.Core.EventSourcing.Aggregates.Attributes;
using Carcass.Data.Core.EventSourcing.Aggregates.Helpers;
using Carcass.Data.Core.EventSourcing.Aggregates.Repositories.Abstracts;
using Carcass.Data.Core.EventSourcing.Aggregates.ResolutionStrategies.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.Core.EventSourcing.Snapshotting.Abstracts;
using Carcass.Data.Core.EventSourcing.Snapshotting.Repositories.Abstracts;
using Carcass.Data.EventStoreDb.Aggregates.Extensions;
using Carcass.Data.EventStoreDb.Aggregates.ResolutionStrategies.Extensions;
using Carcass.Data.EventStoreDb.Extensions;
using Carcass.Data.EventStoreDb.Options;
using Carcass.Json.Core.Providers.Abstracts;
using EventStore.Client;
using Microsoft.Extensions.Options;

namespace Carcass.Data.EventStoreDb.Aggregates.Repositories;

public sealed class EventStoreDbAggregateRepository : IAggregateRepository
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IOptions<EventStoreDbOptions> _optionsAccessor;
    private readonly IJsonProvider _jsonProvider;
    private readonly IAggregateNameResolutionStrategy _aggregateNameResolutionStrategy;
    private readonly IDomainEventLocator _domainEventLocator;
    private readonly IDomainEventUpgraderDispatcher _domainEventUpgraderDispatcher;
    private readonly ISnapshotRepository _snapshotRepository;

    public EventStoreDbAggregateRepository(
        IJsonProvider jsonProvider,
        IAggregateNameResolutionStrategy aggregateNameResolutionStrategy,
        ISnapshotRepository snapshotRepository,
        IDomainEventUpgraderDispatcher domainEventUpgraderDispatcher,
        IDomainEventLocator domainEventLocator,
        EventStoreClient eventStoreClient,
        IOptions<EventStoreDbOptions> optionsAccessor
    )
    {
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));
        ArgumentVerifier.NotNull(aggregateNameResolutionStrategy, nameof(aggregateNameResolutionStrategy));
        ArgumentVerifier.NotNull(snapshotRepository, nameof(snapshotRepository));
        ArgumentVerifier.NotNull(domainEventUpgraderDispatcher, nameof(domainEventUpgraderDispatcher));
        ArgumentVerifier.NotNull(domainEventLocator, nameof(domainEventLocator));
        ArgumentVerifier.NotNull(eventStoreClient, nameof(eventStoreClient));

        _jsonProvider = jsonProvider;
        _aggregateNameResolutionStrategy = aggregateNameResolutionStrategy;
        _snapshotRepository = snapshotRepository;
        _domainEventUpgraderDispatcher = domainEventUpgraderDispatcher;
        _domainEventLocator = domainEventLocator;
        _eventStoreClient = eventStoreClient;
        _optionsAccessor = optionsAccessor;
    }

    public async Task SaveAggregateAsync<TAggregate>(
        TAggregate aggregate,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(aggregate, nameof(aggregate));

        EventData[] history = aggregate.History
            .Select(o => new EventData(
                    Uuid.NewUuid(),
                    o.GetType().Name,
                    Encoding.UTF8.GetBytes(_jsonProvider.Serialize(o)),
                    Encoding.UTF8.GetBytes(o.GetType().FullName!)
                )
            ).ToArray();

        if (!history.Any())
            return;

        string aggregateKey = _aggregateNameResolutionStrategy.GetEventStoreDbStreamName(aggregate);

        await _eventStoreClient.AppendToStreamAsync(
            aggregateKey,
            StreamState.Any,
            history,
            cancellationToken: cancellationToken
        );

        EventStoreClient.ReadStreamResult readStreamResult = _eventStoreClient.ReadStreamAsync(
            Direction.Backwards,
            aggregateKey,
            StreamPosition.End,
            1,
            resolveLinkTos: true,
            cancellationToken: cancellationToken
        );

        List<ResolvedEvent>? resolvedEvents = await readStreamResult.GetResolvedEventsAsync(cancellationToken);
        if (resolvedEvents is not null && resolvedEvents.Any())
        {
            long lastResolvedEventNumber = resolvedEvents.First().Event.GetEventNumber();
            AggregateVersionAttribute aggregateVersionAttribute =
                AggregateHelper.GetAggregateVersionAttribute<TAggregate>();
            long takeSnapshotAfterEventsCount = _optionsAccessor.Value.TakeSnapshotAfterEventsCount;
            long eventsMaxCount = _optionsAccessor.Value.EventsMaxCount;

            ISnapshot? snapshot = await _snapshotRepository.LoadSnapshotAsync(aggregateKey, cancellationToken);
            if (
                snapshot?.Payload is not null &&
                snapshot.TakeSnapshotAfterEventsCount == takeSnapshotAfterEventsCount &&
                snapshot.AggregateSchemaVersion == aggregateVersionAttribute.Version
            )
            {
                TAggregate? aggregateFromSnapshot = _jsonProvider.TryDeserialize<TAggregate>(snapshot.Payload);
                if (aggregateFromSnapshot is null)
                    throw new InvalidOperationException(
                        $"Aggregate {typeof(TAggregate).Name} could not be restored from snapshot."
                    );

                if (lastResolvedEventNumber - aggregateFromSnapshot.Version >= takeSnapshotAfterEventsCount)
                {
                    TAggregate aggregateForNewSnapshot = await _eventStoreClient.GetAggregateAsync(
                        _domainEventLocator,
                        _domainEventUpgraderDispatcher,
                        _jsonProvider,
                        aggregateFromSnapshot,
                        aggregateKey,
                        Direction.Forwards,
                        StreamPosition.FromInt64(aggregateFromSnapshot.Version),
                        eventsMaxCount,
                        lastResolvedEventNumber - lastResolvedEventNumber %
                        takeSnapshotAfterEventsCount - aggregateFromSnapshot.Version,
                        cancellationToken
                    );


                    await _snapshotRepository.SaveSnapshotAsync(
                        aggregateKey,
                        aggregateVersionAttribute.Version,
                        _jsonProvider.Serialize(aggregateForNewSnapshot),
                        takeSnapshotAfterEventsCount,
                        cancellationToken
                    );
                }
            }
            else if (lastResolvedEventNumber > takeSnapshotAfterEventsCount)
            {
                TAggregate aggregateForNewSnapshot = await _eventStoreClient.GetAggregateAsync(
                    _domainEventLocator,
                    _domainEventUpgraderDispatcher,
                    _jsonProvider,
                    new TAggregate { Id = aggregate.Id },
                    aggregateKey,
                    Direction.Forwards,
                    StreamPosition.Start,
                    eventsMaxCount,
                    lastResolvedEventNumber - lastResolvedEventNumber % takeSnapshotAfterEventsCount,
                    cancellationToken
                );

                await _snapshotRepository.SaveSnapshotAsync(
                    aggregateKey,
                    aggregateVersionAttribute.Version,
                    _jsonProvider.Serialize(aggregateForNewSnapshot),
                    takeSnapshotAfterEventsCount,
                    cancellationToken
                );
            }
        }
    }

    public async Task<TAggregate> LoadAggregateAsync<TAggregate>(
        Guid aggregateId,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotDefault(aggregateId, nameof(aggregateId));

        TAggregate? aggregate = new();
        AggregateVersionAttribute aggregateVersionAttribute =
            AggregateHelper.GetAggregateVersionAttribute<TAggregate>();
        string aggregateKey = _aggregateNameResolutionStrategy.GetEventStoreDbStreamName(aggregate);
        long takeSnapshotAfterEventsCount = _optionsAccessor.Value.TakeSnapshotAfterEventsCount;
        long eventsMaxCount = _optionsAccessor.Value.EventsMaxCount;

        EventStoreClient.ReadStreamResult latestSlice = _eventStoreClient.ReadStreamAsync(
            Direction.Backwards,
            aggregateKey,
            StreamPosition.End,
            takeSnapshotAfterEventsCount,
            resolveLinkTos: true,
            cancellationToken: cancellationToken
        );
        List<ResolvedEvent>? latestResolvedEvents = await latestSlice.GetResolvedEventsAsync(cancellationToken);

        if (latestResolvedEvents is not null && latestResolvedEvents.Any())
        {
            if (latestResolvedEvents.Count == takeSnapshotAfterEventsCount)
            {
                ISnapshot? snapshot = await _snapshotRepository.LoadSnapshotAsync(aggregateKey, cancellationToken);

                if (snapshot?.Payload is not null &&
                    snapshot.TakeSnapshotAfterEventsCount == takeSnapshotAfterEventsCount &&
                    snapshot.AggregateSchemaVersion == aggregateVersionAttribute.Version
                   )
                {
                    aggregate = _jsonProvider.TryDeserialize<TAggregate>(snapshot.Payload);
                    if (aggregate is null)
                        throw new InvalidOperationException(
                            $"Aggregate {typeof(TAggregate).Name} could not be restored from snapshot."
                        );
                    long nextAggregateVersion = aggregate.Version + 1;
                    long latestResolvedEventNumber = latestResolvedEvents.First().Event.GetEventNumber();
                    long eventsCountAfterSnapshot = latestResolvedEventNumber % takeSnapshotAfterEventsCount;

                    if (eventsCountAfterSnapshot == 0)
                    {
                        if (aggregate.Version == latestResolvedEventNumber)
                            return aggregate;

                        return await _eventStoreClient.GetAggregateAsync(
                            _domainEventLocator,
                            _domainEventUpgraderDispatcher,
                            _jsonProvider,
                            aggregate,
                            aggregateKey,
                            Direction.Forwards,
                            StreamPosition.FromInt64(nextAggregateVersion),
                            eventsMaxCount,
                            cancellationToken
                        );
                    }

                    List<ResolvedEvent> unAppliedEvents = latestResolvedEvents
                        .Take((int) eventsCountAfterSnapshot)
                        .Reverse()
                        .ToList();

                    if (nextAggregateVersion == unAppliedEvents.First().Event.GetEventNumber())
                    {
                        aggregate.ApplyResolvedEvents(
                            unAppliedEvents,
                            _domainEventLocator,
                            _domainEventUpgraderDispatcher,
                            _jsonProvider
                        );

                        return aggregate;
                    }

                    return await _eventStoreClient.GetAggregateAsync(
                        _domainEventLocator,
                        _domainEventUpgraderDispatcher,
                        _jsonProvider,
                        aggregate,
                        aggregateKey,
                        Direction.Forwards,
                        StreamPosition.FromInt64(nextAggregateVersion),
                        eventsMaxCount,
                        cancellationToken
                    );
                }

                return await _eventStoreClient.GetAggregateAsync(
                    _domainEventLocator,
                    _domainEventUpgraderDispatcher,
                    _jsonProvider,
                    aggregate,
                    aggregateKey,
                    Direction.Forwards,
                    StreamPosition.Start,
                    eventsMaxCount,
                    cancellationToken
                );
            }

            latestResolvedEvents.Reverse();
            aggregate.ApplyResolvedEvents(
                latestResolvedEvents,
                _domainEventLocator,
                _domainEventUpgraderDispatcher,
                _jsonProvider
            );
        }

        return aggregate;
    }
}