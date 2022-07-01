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

using System.Text;
using Carcass.Core;
using Carcass.Core.Extensions;
using Carcass.Data.Core.Aggregates.ResolutionStrategies.Abstracts;

namespace Carcass.Data.Core.Aggregates.ResolutionStrategies;

public sealed class CapitalizedAggregateNameResolutionStrategy : IAggregateNameResolutionStrategy
{
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