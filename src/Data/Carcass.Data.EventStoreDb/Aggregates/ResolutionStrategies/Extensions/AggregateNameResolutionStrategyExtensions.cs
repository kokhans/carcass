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
using Carcass.Data.Core.EventSourcing.Aggregates.ResolutionStrategies.Abstracts;

namespace Carcass.Data.EventStoreDb.Aggregates.ResolutionStrategies.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="IAggregateNameResolutionStrategy" /> interface to assist
///     in resolving aggregate names for use with EventStoreDB stream naming conventions.
/// </summary>
public static class AggregateNameResolutionStrategyExtensions
{
    /// <summary>
    ///     Generates the Event Store DB stream name for a given aggregate based on its type name and identifier.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate. Must inherit from <see cref="Aggregate" />.</typeparam>
    /// <param name="aggregateNameResolutionStrategy">
    ///     The strategy responsible for resolving the aggregate's name.
    /// </param>
    /// <param name="aggregate">The instance of the aggregate for which the stream name is generated.</param>
    /// <returns>The constructed Event Store DB stream name in the format "{AggregateName}-{AggregateId}".</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="aggregateNameResolutionStrategy" /> or <paramref name="aggregate" /> is null.
    /// </exception>
    public static string GetEventStoreDbStreamName<TAggregate>(
        this IAggregateNameResolutionStrategy aggregateNameResolutionStrategy,
        TAggregate aggregate
    ) where TAggregate : Aggregate
    {
        ArgumentVerifier.NotNull(aggregateNameResolutionStrategy, nameof(aggregateNameResolutionStrategy));
        ArgumentVerifier.NotNull(aggregate, nameof(aggregate));

        return $"{aggregateNameResolutionStrategy.GetAggregateName(aggregate.GetType().Name)}-{aggregate.Id}";
    }

    /// <summary>
    ///     Resolves the persistent subscription stream name for the specified aggregate type name using the provided strategy.
    /// </summary>
    /// <param name="aggregateNameResolutionStrategy">
    ///     The aggregate name resolution strategy to use for resolving the stream
    ///     name.
    /// </param>
    /// <param name="aggregateTypeName">The type name of the aggregate for which to resolve the stream name.</param>
    /// <returns>The persistent subscription stream name for the specified aggregate type.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="aggregateNameResolutionStrategy" /> or <paramref name="aggregateTypeName" /> is null.
    /// </exception>
    public static string GetEventStoreDbPersistentSubscriptionStreamName(
        this IAggregateNameResolutionStrategy aggregateNameResolutionStrategy,
        string aggregateTypeName
    )
    {
        ArgumentVerifier.NotNull(aggregateNameResolutionStrategy, nameof(aggregateNameResolutionStrategy));
        ArgumentVerifier.NotNull(aggregateTypeName, nameof(aggregateTypeName));

        return $"$ce-{aggregateNameResolutionStrategy.GetAggregateName(aggregateTypeName)}";
    }
}