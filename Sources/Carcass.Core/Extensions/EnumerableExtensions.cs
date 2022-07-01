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

using System.Collections.ObjectModel;

namespace Carcass.Core.Extensions;

public static class EnumerableExtensions
{
    public static T? Random<T>(this IList<T>? source)
    {
        if (source is null || !source.Any())
            return default;

        Random random = new();
        int index = random.Next(0, source.Count);

        return source[index];
    }

    public static int? Multiply(this IList<int>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Aggregate(1, (lhs, rhs) => lhs * rhs);
    }

    public static bool? IsSequential(this IList<byte>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Zip(source.Skip(1), (lhs, rhs) => lhs + 1 == rhs).All(b => b);
    }

    public static bool? IsSequential(this IList<int>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Zip(source.Skip(1), (lhs, rhs) => lhs + 1 == rhs).All(b => b);
    }

    public static bool? IsSequential(this IList<long>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Zip(source.Skip(1), (lhs, rhs) => lhs + 1 == rhs).All(b => b);
    }

    public static ReadOnlyCollection<T> AsReadOnlyCollection<T>(this IList<T> source)
    {
        ArgumentVerifier.NotNull(source, nameof(source));

        return new ReadOnlyCollection<T>(source);
    }
}