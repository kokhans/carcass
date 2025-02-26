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

using Carcass.Core.Extensions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Carcass.Core.Helpers;

/// <summary>
///     Provides utility methods to generate URL-friendly slugs from collections of strings and GUIDs.
/// </summary>
public sealed class SlugHelper
{
    /// <summary>
    ///     Generates a slug string by combining a specified GUID with a list of strings and converting the result to a slug
    ///     format.
    /// </summary>
    /// <param name="guid">The GUID to include in the slug generation process.</param>
    /// <param name="source">A list of strings to compose the slug. Can be null or empty.</param>
    /// <returns>A slug string created from the input GUID and strings, or null if the source list is null or empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided GUID is null.</exception>
    public static string? GetSlug(Guid guid, IList<string>? source)
    {
        if (source is null || !source.Any())
            return null;

        return source
            .Concat([ShortGuid.Encode(guid)])
            .ToList()
            .ToCommaSeparated()
            .ToSlug();
    }
}