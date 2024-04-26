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
using System.Text.RegularExpressions;

// ReSharper disable UnusedMember.Global

namespace Carcass.Core.Extensions;

/// <summary>
///     Provides extension methods for manipulating and formatting strings.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    ///     Converts the input string to its ASCII representation by replacing accented and special characters
    ///     with their closest ASCII equivalents.
    /// </summary>
    /// <param name="value">The input string to be converted. Can be null or empty.</param>
    /// <returns>The ASCII representation of the input string, or null if the input is null.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the input string is null if appropriate null-checks are not in
    ///     place.
    /// </exception>
    public static string? ToAscii(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        StringBuilder sb = new();

        value
            .ToCharArray()
            .ToList()
            .ForEach(c => sb.Append(c.ToAscii()));

        return sb.ToString();
    }

    /// <summary>
    ///     Splits a string into its constituent words based on capitalization, where each capitalized segment becomes a
    ///     separate word.
    /// </summary>
    /// <param name="value">The input string to be split. Can be null or empty.</param>
    /// <returns>
    ///     An enumerable collection of strings containing the words extracted from the input string, or an empty
    ///     enumeration if the input is null or empty.
    /// </returns>
    /// <exception cref="RegexMatchTimeoutException">Thrown if a regular expression match exceeds its timeout interval.</exception>
    public static IEnumerable<string> SplitCapitalizedWords(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        foreach (Match m in CapitalizedWordsRegex().Matches(value))
            yield return m.Value;
    }

    /// <summary>
    ///     Converts the first letter of the input string to uppercase and converts the remaining characters to lowercase.
    /// </summary>
    /// <param name="value">The input string to modify. If the string is null or empty, it will be returned as is.</param>
    /// <returns>
    ///     A string with the first letter converted to uppercase and the remaining characters converted to lowercase.
    ///     Returns null if the input is null, or returns the original string if it is empty.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the substring operation tries to access an invalid range of characters in the string.
    /// </exception>
    public static string? ToUpperFirstLetter(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..].ToLowerInvariant();
    }

    /// <summary>
    ///     Converts the first letter of the given string to lowercase while keeping the rest of the string unchanged.
    /// </summary>
    /// <param name="value">The input string to process. Can be null or empty.</param>
    /// <returns>
    ///     A new string with the first letter converted to lowercase if applicable.
    ///     Returns the original string if it is null or empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the input string is unexpectedly null during processing. (Not
    ///     applicable in the current implementation.)
    /// </exception>
    public static string? ToLowerFirstLetter(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    ///     Converts a string to snake_case format by replacing camelCase or PascalCase patterns with underscores between
    ///     words.
    /// </summary>
    /// <param name="value">
    ///     The input string to convert. If the input is null or consists solely of white-space characters, it
    ///     is returned unchanged.
    /// </param>
    /// <returns>
    ///     A new string in snake_case format, or the original value if it was null or empty.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if any regex operation produces an unexpected result during the transformation.
    /// </exception>
    public static string? ToSnakeCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return RemoveLeadingUnderscoreRegex().Match(value) +
               CamelToSnakeCaseRegex().Replace(value, "$1_$2").ToLower();
    }

    /// <summary>
    ///     Appends a trailing slash to the input string if it doesn't already end with one.
    /// </summary>
    /// <param name="value">The input string to process. Can be null or empty.</param>
    /// <returns>
    ///     The input string with a trailing slash appended, or the original string if it already ends with a slash, or
    ///     null if the input is null or empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the input string is null or empty.</exception>
    public static string? AppendTrailingSlash(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return value.EndsWith('/')
            ? value
            : $"{value}/";
    }

    /// <summary>
    ///     Converts a list of strings into a single comma-separated string.
    /// </summary>
    /// <param name="source">The list of strings to be joined. Can be null or empty.</param>
    /// <returns>
    ///     A comma-separated string representation of the list, or null if the input is null or empty.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> is null.</exception>
    public static string? ToCommaSeparated(this IList<string>? source)
    {
        if (source is null || !source.Any())
            return null;

        StringBuilder stringBuilder = new();
        int count = source.Count;

        for (int i = 0; i < count; i++)
        {
            stringBuilder.Append(source[i]);
            if (i != count - 1)
                stringBuilder.Append(',');
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    ///     Converts the input string into a URL-friendly slug, enforcing
    ///     lowercased alphanumeric characters separated by dashes.
    /// </summary>
    /// <param name="value">The input string to be converted into a slug. Can be null or empty.</param>
    /// <param name="maxLength">The maximum allowed length for the slug. Defaults to 128.</param>
    /// <returns>
    ///     Returns the slugified version of the input string. Returns null if the input is null or whitespace.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the provided <paramref name="maxLength" /> is less than 1.
    /// </exception>
    public static string? ToSlug(this string? value, int maxLength = 128)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        int length = value.Length;
        bool prevDash = false;
        StringBuilder stringBuilder = new(length);

        for (int i = 0; i < length; i++)
        {
            char c = value[i];
            switch (c)
            {
                case >= 'a' and <= 'z':
                case >= '0' and <= '9':
                    stringBuilder.Append(c);
                    prevDash = false;
                    break;
                case >= 'A' and <= 'Z':
                    stringBuilder.Append((char) (c | 32));
                    prevDash = false;
                    break;
                case ' ' or ',' or '.' or '/' or '\\' or '-' or '_' or '=':
                {
                    if (!prevDash && stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('-');
                        prevDash = true;
                    }

                    break;
                }
                default:
                {
                    if (c >= 128)
                    {
                        int prevLength = stringBuilder.Length;
                        stringBuilder.Append(c.ToAscii());
                        if (prevLength != stringBuilder.Length)
                            prevDash = false;
                    }

                    break;
                }
            }

            if (i == maxLength)
                break;
        }

        return prevDash ? stringBuilder.ToString()[..(stringBuilder.Length - 1)] : stringBuilder.ToString();
    }

    /// <summary>
    ///     Creates a regular expression that matches one or more leading underscores in a string.
    /// </summary>
    /// <returns>
    ///     A <see cref="Regex" /> instance that matches strings with leading underscores.
    /// </returns>
    /// <exception cref="RegexMatchTimeoutException">
    ///     Thrown when a match timeout occurs while evaluating the regular expression against an input string.
    /// </exception>
    [GeneratedRegex("^_+")]
    private static partial Regex RemoveLeadingUnderscoreRegex();

    /// <summary>
    ///     Matches camel case string patterns to transform them into snake case.
    /// </summary>
    /// <returns>
    ///     A <see cref="Regex" /> pattern that matches the transition from a lowercase letter or digit
    ///     to an uppercase letter, enabling conversion from camel case to snake case.
    /// </returns>
    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex CamelToSnakeCaseRegex();

    /// <summary>
    ///     Matches and retrieves groups of capitalized words in a string where each word starts with an uppercase letter
    ///     followed by lowercase letters.
    /// </summary>
    /// <returns>
    ///     A <see cref="Regex" /> instance configured to match sequences of words with uppercase beginnings.
    /// </returns>
    [GeneratedRegex("([A-Z][a-z]+)")]
    private static partial Regex CapitalizedWordsRegex();
}