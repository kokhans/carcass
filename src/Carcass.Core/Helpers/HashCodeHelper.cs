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

namespace Carcass.Core.Helpers;

/// <summary>
///     Provides helper methods for generating deterministic hash codes.
/// </summary>
public static class HashCodeHelper
{
    /// <summary>
    ///     Computes a deterministic hash code for the specified string.
    /// </summary>
    /// <param name="value">The input string for which the hash code will be computed. Can be null or whitespace.</param>
    /// <returns>
    ///     An <see cref="int" /> representing the hash code if the input is not null or whitespace; otherwise, returns null.
    /// </returns>
    /// <exception cref="System.OverflowException">
    ///     Thrown if arithmetic overflow occurs during hash computation.
    /// </exception>
    public static int? GetDeterministicHashCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        unchecked
        {
            int hash1 = (5381 << 16) + 5381;
            int hash2 = hash1;

            for (int i = 0; i < value.Length; i += 2)
            {
                hash1 = ((hash1 << 5) + hash1) ^ value[i];

                if (i == value.Length - 1)
                    break;

                hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
            }

            return hash1 + hash2 * 1566083941;
        }
    }
}