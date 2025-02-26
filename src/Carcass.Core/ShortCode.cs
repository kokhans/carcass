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

namespace Carcass.Core;

/// <summary>
///     Provides functionality for generating short, alphanumeric codes of specified length.
/// </summary>
public static class ShortCode
{
    /// <summary>
    ///     Represents a constant string containing characters, which include uppercase English alphabets
    ///     and numeric digits, used for generating short codes or similar utilities.
    /// </summary>
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    ///     Represents a pseudo-random number generator used to generate random numbers.
    /// </summary>
    /// <remarks>
    ///     Typically used for generating random numbers or selecting random elements
    ///     in various operations where a secure random number is not required.
    /// </remarks>
    /// <seealso cref="System.Random" />
    private static readonly Random Random = new();

    /// <summary>
    ///     Generates a random short code consisting of uppercase letters and digits with a specified length.
    /// </summary>
    /// <param name="length">The length of the short code to generate. Must be greater than 1.</param>
    /// <returns>A string representing the generated short code.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified length is less than or equal to 1.</exception>
    public static string NewShortCode(int length)
    {
        ArgumentVerifier.Requires(length > 1, "Length must be greater than 0.");

        StringBuilder stringBuilder = new(length);

        for (int i = 0; i < length; i++)
            stringBuilder.Append(Characters[Random.Next(Characters.Length)]);

        return stringBuilder.ToString();
    }
}