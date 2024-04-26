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

using Carcass.Core;
using Carcass.Data.Core.EventSourcing.Aggregates.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.EventStoreDb.Aggregates.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using EventStore.Client;

namespace Carcass.Data.EventStoreDb.Extensions;

/// <summary>
///     Provides extension methods for the EventStoreClient to facilitate handling of resolved events and aggregates.
/// </summary>
public static class EventStoreClientExtensions
{
    /// <summary>
    ///     Retrieves a list of resolved events from the specified <see cref="EventStoreClient.ReadStreamResult" />
    ///     asynchronously.
    /// </summary>
    /// <param name="readStreamResult">
    ///     The read stream result representing the events stream to retrieve resolved events from.
    /// </param>
    /// <param name="cancellationToken">
    ///     An optional <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result is a list of <see cref="ResolvedEvent" /> objects,
    ///     or null if the stream is not found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="readStreamResult" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public static async Task<List<ResolvedEvent>?> GetResolvedEventsAsync(
        this EventStoreClient.ReadStreamResult readStreamResult,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(readStreamResult, nameof(readStreamResult));

        try
        {
            return await readStreamResult.ToListAsync(cancellationToken);
        }
        catch (StreamNotFoundException)
        {
        }

        return null;
    }

    /// <summary>
    ///     Retrieves an aggregate from the Event Store by applying domain events found in a specified stream.
    /// </summary>
    /// <typeparam name="TAggregate">
    ///     The type of the aggregate being retrieved, which must inherit from the
    ///     <see cref="Aggregate" /> class.
    /// </typeparam>
    /// <param name="eventStoreClient">The EventStore client through which the stream is accessed.</param>
    /// <param name="domainEventLocator">
    ///     The locator used to identify and map events in the stream to their corresponding
    ///     domain event types.
    /// </param>
    /// <param name="domainEventUpgraderDispatcher">
    ///     The dispatcher responsible for upgrading older versions of domain events to
    ///     their latest versions.
    /// </param>
    /// <param name="jsonProvider">The JSON provider used to deserialize event data from the stream into domain events.</param>
    /// <param name="aggregate">The aggregate instance to which domain events will be applied. A new instance is used if null.</param>
    /// <param name="aggregateKey">The unique identifier of the aggregate, used to locate the stream in the Event Store.</param>
    /// <param name="direction">The direction in which stream events are read. Can be forward or backward.</param>
    /// <param name="streamPosition">The position in the stream from which to start reading events.</param>
    /// <param name="maxCount">The maximum number of events to retrieve in a single request.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The reconstructed aggregate after applying the retrieved domain events.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any required parameter is null.</exception>
    /// <exception cref="StreamNotFoundException">Thrown when the specified stream does not exist in the Event Store.</exception>
    /// <exception cref="InvalidOperationException">Thrown when an error occurs in applying events to the aggregate.</exception>
    public static async Task<TAggregate> GetAggregateAsync<TAggregate>(
        this EventStoreClient eventStoreClient,
        IDomainEventLocator domainEventLocator,
        IDomainEventUpgraderDispatcher domainEventUpgraderDispatcher,
        IJsonProvider jsonProvider,
        TAggregate aggregate,
        string aggregateKey,
        Direction direction,
        StreamPosition streamPosition,
        long maxCount,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new() =>
        await GetAggregateAsync(
            eventStoreClient,
            domainEventLocator,
            domainEventUpgraderDispatcher,
            jsonProvider,
            aggregate,
            aggregateKey,
            direction,
            streamPosition,
            maxCount,
            long.MaxValue,
            cancellationToken
        );

    /// <summary>
    ///     Retrieves an aggregate from the Event Store asynchronously. The method reads events related to the specified
    ///     aggregate,
    ///     processes them using the provided domain event locator, upgrader, and JSON provider, and applies them to the
    ///     aggregate instance.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate to retrieve, which must inherit from <see cref="Aggregate" />.</typeparam>
    /// <param name="eventStoreClient">The instance of <see cref="EventStoreClient" /> to read events from Event Store.</param>
    /// <param name="domainEventLocator">The locator used to find domain events corresponding to the aggregate.</param>
    /// <param name="domainEventUpgraderDispatcher">
    ///     The dispatcher responsible for upgrading domain event versions if
    ///     necessary.
    /// </param>
    /// <param name="jsonProvider">The JSON provider used for serialization and deserialization of domain events.</param>
    /// <param name="aggregate">The aggregate instance to which domain events will be applied.</param>
    /// <param name="aggregateKey">The key or stream name identifying the aggregate in the Event Store.</param>
    /// <param name="direction">The direction to read the stream (forwards or backwards).</param>
    /// <param name="streamPosition">The starting position in the stream from which to begin reading events.</param>
    /// <param name="maxCount">The maximum number of events to retrieve per batch.</param>
    /// <param name="takeCount">The maximum number of events to process in total.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The aggregate with its state reconstructed from the processed domain events.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if any required parameter is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    public static async Task<TAggregate> GetAggregateAsync<TAggregate>(
        this EventStoreClient eventStoreClient,
        IDomainEventLocator domainEventLocator,
        IDomainEventUpgraderDispatcher domainEventUpgraderDispatcher,
        IJsonProvider jsonProvider,
        TAggregate aggregate,
        string aggregateKey,
        Direction direction,
        StreamPosition streamPosition,
        long maxCount,
        long takeCount,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(eventStoreClient, nameof(eventStoreClient));
        ArgumentVerifier.NotNull(domainEventLocator, nameof(domainEventLocator));
        ArgumentVerifier.NotNull(domainEventUpgraderDispatcher, nameof(domainEventUpgraderDispatcher));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));
        ArgumentVerifier.NotNull(aggregate, nameof(aggregate));
        ArgumentVerifier.NotNull(aggregateKey, nameof(aggregateKey));

        long eventsCountToTake = takeCount;
        do
        {
            EventStoreClient.ReadStreamResult readStreamResult = eventStoreClient.ReadStreamAsync(
                direction,
                aggregateKey,
                streamPosition,
                eventsCountToTake > maxCount ? maxCount : takeCount,
                true,
                cancellationToken: cancellationToken
            );
            List<ResolvedEvent>? resolvedEvents = await readStreamResult.GetResolvedEventsAsync(cancellationToken);
            if (resolvedEvents is not null && resolvedEvents.Count != 0)
            {
                aggregate.ApplyResolvedEvents(
                    resolvedEvents,
                    domainEventLocator,
                    domainEventUpgraderDispatcher,
                    jsonProvider
                );

                eventsCountToTake -= resolvedEvents.Count;
                if (eventsCountToTake <= 0)
                    continue;

                await using IAsyncEnumerator<ResolvedEvent> asyncEnumerator =
                    readStreamResult.GetAsyncEnumerator(cancellationToken);
                eventsCountToTake = await asyncEnumerator.MoveNextAsync()
                    ? eventsCountToTake
                    : 0;
            }
            else
            {
                eventsCountToTake = 0;
            }
        } while (eventsCountToTake > 0);

        return aggregate;
    }
}