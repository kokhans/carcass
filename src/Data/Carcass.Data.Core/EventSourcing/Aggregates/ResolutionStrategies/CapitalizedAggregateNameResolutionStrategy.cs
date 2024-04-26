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
using Carcass.Core.Extensions;
using Carcass.Data.Core.EventSourcing.Aggregates.ResolutionStrategies.Abstracts;

namespace Carcass.Data.Core.EventSourcing.Aggregates.ResolutionStrategies;

/// <summary>
///     A strategy for resolving aggregate names by removing the word "Aggregate"
///     and combining the component words of a type name that are split based on capitalization.
/// </summary>
public sealed class CapitalizedAggregateNameResolutionStrategy : IAggregateNameResolutionStrategy
{
    /// <summary>
    ///     Resolves the aggregate name by processing the given aggregate type name.
    ///     Removes the term "Aggregate" and joins split capitalized words.
    /// </summary>
    /// <param name="aggregateTypeName">The name of the aggregate type to be resolved into an aggregate name.</param>
    /// <returns>A processed string representing the resolved aggregate name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="aggregateTypeName" /> is null.</exception>
    public string GetAggregateName(string aggregateTypeName)
    {
        ArgumentVerifier.NotNull(aggregateTypeName, nameof(aggregateTypeName));

        List<string> words = aggregateTypeName
            .Replace("Aggregate", string.Empty)
            .SplitCapitalizedWords()
            .ToList();

        StringBuilder stringBuilder = new();

        foreach (string word in words)
            stringBuilder.Append(word);

        return stringBuilder.ToString();
    }
}