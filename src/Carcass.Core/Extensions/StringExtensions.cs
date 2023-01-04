// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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

namespace Carcass.Core.Extensions;

public static class StringExtensions
{
    public static IEnumerable<string> SplitCapitalizedWords(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        foreach (Match m in Regex.Matches(value, @"([A-Z][a-z]+)"))
            yield return m.Value;
    }

    public static string? ToUpperFirstLetter(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..].ToLowerInvariant();
    }

    public static string? ToLowerFirstLetter(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    public static string? ToSnakeCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return Regex.Match(value, @"^_+") +
               Regex.Replace(value, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    public static string? AppendTrailingSlash(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        return value.EndsWith('/')
            ? value
            : $"{value}/";
    }

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
                        stringBuilder.Append(c.RemapInternationalCharToAscii());
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
}