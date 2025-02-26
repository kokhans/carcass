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

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable UnusedMember.Global

namespace Carcass.Core.Extensions;

/// <summary>
///     Provides a set of extension methods for IEnumerable, IList, IDictionary, and related collection types.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Retrieves the value associated with the specified key. If the key is not found, adds the specified value to the
    ///     dictionary and returns it.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary in which to search for the key or add the value.</param>
    /// <param name="key">The key to locate or add in the dictionary.</param>
    /// <param name="value">The value to add if the key is not found in the dictionary.</param>
    /// <returns>The value associated with the key if it exists in the dictionary; otherwise, the newly added value.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the dictionary or key is null.</exception>
    public static TValue? GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue? value) where TKey : notnull
    {
        ref TValue? valueRef = ref CollectionsMarshal.GetValueRefOrAddDefault(dictionary, key, out bool exists);
        if (exists)
            return valueRef;

        valueRef = value;

        return value;
    }

    /// <summary>
    ///     Attempts to update the value associated with the specified key in the dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="dictionary">The dictionary to perform the update operation on.</param>
    /// <param name="key">The key whose value should be updated.</param>
    /// <param name="value">The new value to associate with the specified key.</param>
    /// <returns>
    ///     <c>true</c> if the value was successfully updated; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if the dictionary is null.</exception>
    public static bool TryUpdate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        TValue value) where TKey : notnull
    {
        ref TValue valueRef = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);
        if (Unsafe.IsNullRef(ref valueRef))
            return false;

        valueRef = value;

        return true;
    }

    /// <summary>
    ///     Selects a random element from the given list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="source">The list from which to select a random element.</param>
    /// <returns>
    ///     The randomly selected element, or the default value of the type
    ///     if the list is null or empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    public static T? Random<T>(this IList<T>? source)
    {
        if (source is null || !source.Any())
            return default;

        Random random = new();
        int index = random.Next(0, source.Count);

        return source[index];
    }

    /// <summary>
    ///     Multiplies all integers in the given list and returns the product.
    /// </summary>
    /// <param name="source">The list of integers to be multiplied. Can be null or empty.</param>
    /// <returns>
    ///     The product of all integers in the list, or null if the list is null or empty.
    /// </returns>
    /// <exception cref="OverflowException">Thrown if the product exceeds the range of an integer.</exception>
    public static int? Multiply(this IList<int>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Aggregate(1, (lhs, rhs) => lhs * rhs);
    }

    /// <summary>
    ///     Determines whether the elements in the provided list follow a sequential order, where each element is incremented
    ///     by one relative to the previous element.
    /// </summary>
    /// <param name="source">The list of elements to check for sequential order. Accepts null or an empty list.</param>
    /// <returns>
    ///     Returns <c>true</c> if all elements in the list are in sequential order, <c>false</c> if they are not, and
    ///     <c>null</c> if the list is null or empty.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown if the list is null in cases where validation is added
    ///     externally.
    /// </exception>
    public static bool? IsSequential(this IList<byte>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Zip(source.Skip(1), (lhs, rhs) => lhs + 1 == rhs).All(b => b);
    }

    /// <summary>
    ///     Determines whether the elements in the given list of integers are in sequential order.
    /// </summary>
    /// <param name="source">The list of integers to check. If null or empty, returns null.</param>
    /// <returns>
    ///     True if all elements in the list are in a sequential, incremental order.
    ///     False if elements are not sequential. Null if the input list is null or empty.
    /// </returns>
    public static bool? IsSequential(this IList<int>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Zip(source.Skip(1), (lhs, rhs) => lhs + 1 == rhs).All(b => b);
    }

    /// <summary>
    ///     Determines whether the elements in the provided list are sequentially ordered, with each element being
    ///     incremented by 1 from the previous element.
    /// </summary>
    /// <param name="source">The list of long integers to check for sequential ordering.</param>
    /// <returns>
    ///     True if the elements in the list are sequentially ordered; false if not. Null if the list is null or empty.
    /// </returns>
    public static bool? IsSequential(this IList<long>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source.Zip(source.Skip(1), (lhs, rhs) => lhs + 1 == rhs).All(b => b);
    }

    /// <summary>
    ///     Converts an <see cref="IList{T}" /> to a <see cref="ReadOnlyCollection{T}" /> to provide a read-only wrapper for
    ///     the list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="source">The source list to convert to a read-only collection.</param>
    /// <returns>A <see cref="ReadOnlyCollection{T}" /> that wraps the provided list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    public static ReadOnlyCollection<T> AsReadOnlyCollection<T>(this IList<T> source)
    {
        ArgumentVerifier.NotNull(source, nameof(source));

        return new ReadOnlyCollection<T>(source);
    }

    /// <summary>
    ///     Converts an <see cref="IDictionary{TKey, TValue}" /> into a <see cref="ReadOnlyDictionary{TKey, TValue}" />.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary. Keys must not be null.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    /// <param name="source">The dictionary to convert to a read-only dictionary. Must not be null.</param>
    /// <returns>A read-only wrapper around the specified dictionary.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source" /> is null.</exception>
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(
        this IDictionary<TKey, TValue> source
    ) where TKey : notnull
    {
        ArgumentVerifier.NotNull(source, nameof(source));

        return new ReadOnlyDictionary<TKey, TValue>(source);
    }
}