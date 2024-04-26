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

using Carcass.Core.Helpers;
using Carcass.Data.Core.EventSourcing.Aggregates.Abstracts;
using Carcass.Data.Core.EventSourcing.Aggregates.Attributes;

namespace Carcass.Data.Core.EventSourcing.Aggregates.Helpers;

/// <summary>
///     A helper class for operations related to aggregates in the context of event sourcing.
///     This class provides functionality for retrieving metadata attributes associated with aggregates.
/// </summary>
public static class AggregateHelper
{
    /// <summary>
    ///     Retrieves the <see cref="AggregateVersionAttribute" /> associated with a specified aggregate type.
    /// </summary>
    /// <typeparam name="TAggregate">
    ///     The type of the aggregate for which the version attribute is to be retrieved. Must inherit
    ///     from <see cref="Aggregate" />.
    /// </typeparam>
    /// <returns>The <see cref="AggregateVersionAttribute" /> associated with the specified aggregate type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the specified aggregate type does not have an
    ///     <see cref="AggregateVersionAttribute" /> applied.
    /// </exception>
    public static AggregateVersionAttribute GetAggregateVersionAttribute<TAggregate>() where TAggregate : Aggregate
    {
        AggregateVersionAttribute? aggregateVersionAttribute =
            typeof(TAggregate).GetAttribute<AggregateVersionAttribute>();
        if (aggregateVersionAttribute is null)
            throw new InvalidOperationException(
                $"Aggregate {typeof(TAggregate).Name} has no attribute {nameof(AggregateVersionAttribute)}."
            );

        return aggregateVersionAttribute;
    }
}