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

/// <summary>
///     A repository implementation for managing and persisting aggregates using EventStoreDB.
///     Provides methods to save and load aggregates, leveraging event sourcing patterns.
/// </summary>
public sealed class EventStoreDbAggregateRepository : IAggregateRepository
{
    /// <summary>
    ///     Defines the strategy used to resolve and determine the name of an aggregate for operations related to the Event
    ///     Store database.
    /// </summary>
    private readonly IAggregateNameResolutionStrategy _aggregateNameResolutionStrategy;

    /// <summary>
    ///     A private, read-only dependency that facilitates locating domain events within the event sourcing infrastructure.
    /// </summary>
    /// <remarks>
    ///     This member is injected into the repository and is used to determine and locate the appropriate domain events
    ///     during operations related to event retrieval and processing.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown during construction of <see cref="EventStoreDbAggregateRepository" /> if a valid instance is not provided.
    /// </exception>
    private readonly IDomainEventLocator _domainEventLocator;

    /// <summary>
    ///     A dispatcher responsible for handling the upgrading of domain events
    ///     to the latest versions, ensuring compatibility with the current system.
    /// </summary>
    /// <remarks>
    ///     This instance is used to apply any defined transformations or upgrades
    ///     to domain events as they are retrieved, maintaining backward compatibility
    ///     for changes in domain event schemas or structures.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the dispatcher instance is not initialized or provided.
    /// </exception>
    private readonly IDomainEventUpgraderDispatcher _domainEventUpgraderDispatcher;

    /// <summary>
    ///     An instance of <see cref="EventStoreClient" /> used for interacting with an EventStore database.
    /// </summary>
    /// <remarks>
    ///     This client facilitates operations such as appending events to streams, reading stream data, and other
    ///     event store interactions within the context of aggregate repositories.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the EventStoreClient instance is not provided during the repository initialization.
    /// </exception>
    private readonly EventStoreClient _eventStoreClient;

    /// <summary>
    ///     Provides JSON serialization and deserialization functionality for handling data.
    /// </summary>
    /// <remarks>
    ///     This field is used to serialize and deserialize objects to and from JSON, facilitating data storage
    ///     and retrieval in the context of event sourcing within the repository.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if the injection of the JSON provider is null.</exception>
    private readonly IJsonProvider _jsonProvider;

    /// <summary>
    ///     Provides access to the configuration options for EventStoreDb through dependency injection.
    /// </summary>
    /// <remarks>
    ///     The _optionsAccessor field is used to obtain the current <see cref="EventStoreDbOptions" />
    ///     configured for the application. These options may include various settings such as snapshot
    ///     threshold or maximum number of events allowed.
    /// </remarks>
    /// <example>
    ///     This allows accessing configuration settings within the repository without hardcoding values,
    ///     enabling flexibility and maintainability for changes in the application's configuration.
    /// </example>
    /// <seealso cref="EventStoreDbOptions" />
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the supplied IOptions instance is null during construction of the repository.
    /// </exception>
    private readonly IOptions<EventStoreDbOptions> _optionsAccessor;

    /// <summary>
    ///     Represents the repository responsible for managing snapshot persistence
    ///     and retrieval operations in the context of event-sourced aggregates.
    /// </summary>
    /// <remarks>
    ///     This repository is utilized to interact with snapshots in order to
    ///     enhance the performance and efficiency of rebuilding aggregate state
    ///     from event streams.
    /// </remarks>
    private readonly ISnapshotRepository _snapshotRepository;

    /// <summary>
    ///     Represents the repository for EventStoreDB aggregates, providing functionality for loading and saving aggregate
    ///     state
    ///     and associated domain events using EventStoreDB.
    /// </summary>
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

    /// <summary>
    ///     Saves the changes recorded by an aggregate to the Event Store database.
    /// </summary>
    /// <typeparam name="TAggregate">
    ///     The type of the aggregate to save, which must inherit from <see cref="Aggregate" /> and have a parameterless
    ///     constructor.
    /// </typeparam>
    /// <param name="aggregate">
    ///     The aggregate instance containing the changes to be persisted.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests, which can terminate the operation prematurely.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="aggregate" /> argument is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the provided <paramref name="cancellationToken" />.
    /// </exception>
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

        if (history.Length == 0)
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
            true,
            cancellationToken: cancellationToken
        );

        List<ResolvedEvent>? resolvedEvents = await readStreamResult.GetResolvedEventsAsync(cancellationToken);
        if (resolvedEvents is not null && resolvedEvents.Count != 0)
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
                    new TAggregate {Id = aggregate.Id},
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

    /// <summary>
    ///     Loads an aggregate of the specified type by its unique identifier from the Event Store database.
    /// </summary>
    /// <typeparam name="TAggregate">
    ///     The type of the aggregate to load. Must be a subclass of <see cref="Aggregate" /> and have
    ///     a parameterless constructor.
    /// </typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate to load.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The loaded aggregate of type <typeparamref name="TAggregate" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="aggregateId" /> is not provided.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided
    ///     <paramref name="cancellationToken" />.
    /// </exception>
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
            true,
            cancellationToken: cancellationToken
        );
        List<ResolvedEvent>? latestResolvedEvents = await latestSlice.GetResolvedEventsAsync(cancellationToken);

        if (latestResolvedEvents is null || latestResolvedEvents.Count == 0)
            return aggregate;

        if (latestResolvedEvents.Count == takeSnapshotAfterEventsCount)
        {
            ISnapshot? snapshot = await _snapshotRepository.LoadSnapshotAsync(aggregateKey, cancellationToken);

            if (snapshot?.Payload is null ||
                snapshot.TakeSnapshotAfterEventsCount != takeSnapshotAfterEventsCount ||
                snapshot.AggregateSchemaVersion != aggregateVersionAttribute.Version)
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

            if (nextAggregateVersion != unAppliedEvents.First().Event.GetEventNumber())
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
            aggregate.ApplyResolvedEvents(
                unAppliedEvents,
                _domainEventLocator,
                _domainEventUpgraderDispatcher,
                _jsonProvider
            );

            return aggregate;
        }

        latestResolvedEvents.Reverse();
        aggregate.ApplyResolvedEvents(
            latestResolvedEvents,
            _domainEventLocator,
            _domainEventUpgraderDispatcher,
            _jsonProvider
        );

        return aggregate;
    }
}