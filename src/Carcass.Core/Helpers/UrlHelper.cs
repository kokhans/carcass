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

using System.Web;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Carcass.Core.Helpers;

/// <summary>
///     Provides helper methods for working with URLs, such as decoding URL-encoded strings.
/// </summary>
public static class UrlHelper
{
    /// <summary>
    ///     Decodes a URL-encoded string, replacing URL-encoded values with their corresponding characters.
    /// </summary>
    /// <param name="value">The URL-encoded string to decode. Can be null or empty.</param>
    /// <returns>
    ///     A decoded string if the input is not null or empty; otherwise, null.
    /// </returns>
    /// <exception cref="System.ArgumentException">
    ///     Thrown if an error occurs during the decoding process.
    /// </exception>
    public static string? Decode(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : HttpUtility.UrlDecode(value).Replace(' ', '+');
}