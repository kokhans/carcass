// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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
using Carcass.Data.Core.Aggregates.Abstracts;
using Carcass.Data.Core.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.EventStoreDb.Aggregates.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using EventStore.Client;

namespace Carcass.Data.EventStoreDb.Extensions;

public static class EventStoreClientExtensions
{
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
    ) where TAggregate : Aggregate, new()
    {
        return await GetAggregateAsync(
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
    }

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
                resolveLinkTos: true,
                cancellationToken: cancellationToken
            );
            List<ResolvedEvent>? resolvedEvents = await readStreamResult.GetResolvedEventsAsync(cancellationToken);
            if (resolvedEvents is not null && resolvedEvents.Any())
            {
                aggregate.ApplyResolvedEvents(
                    resolvedEvents,
                    domainEventLocator,
                    domainEventUpgraderDispatcher,
                    jsonProvider
                );

                eventsCountToTake -= resolvedEvents.Count;
                if (eventsCountToTake > 0)
                    eventsCountToTake = await readStreamResult.GetAsyncEnumerator(cancellationToken).MoveNextAsync()
                        ? eventsCountToTake
                        : 0;
            }
            else
                eventsCountToTake = 0;
        } while (eventsCountToTake > 0);

        return aggregate;
    }
}