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

using Carcass.Core;
using Carcass.Data.Core.Aggregates.Abstracts;
using Carcass.Data.Core.Aggregates.ResolutionStrategies.Abstracts;

namespace Carcass.Data.EventStoreDb.Aggregates.ResolutionStrategies.Extensions;

public static class AggregateNameResolutionStrategyExtensions
{
    public static string GetEventStoreDbStreamName<TAggregate>(
        this IAggregateNameResolutionStrategy aggregateNameResolutionStrategy,
        TAggregate aggregate
    ) where TAggregate : Aggregate
    {
        ArgumentVerifier.NotNull(aggregateNameResolutionStrategy, nameof(aggregateNameResolutionStrategy));
        ArgumentVerifier.NotNull(aggregate, nameof(aggregate));

        return $"{aggregateNameResolutionStrategy.GetAggregateName(aggregate.GetType().Name)}-{aggregate.Id}";
    }

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