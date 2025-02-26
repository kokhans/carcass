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

using System.Text.Json;
using System.Text.Json.Serialization;
using Carcass.Core;

namespace Carcass.Json.SystemTextJson.Converters;

/// <summary>
///     A custom JSON converter for the <see cref="DateOnly" /> type, allowing
///     serialization and deserialization of <see cref="DateOnly" /> instances
///     to and from JSON using a specified or default date format.
/// </summary>
public sealed class DateOnlyJsonConverter(string? serializationFormat) : JsonConverter<DateOnly>
{
    /// <summary>
    ///     Specifies the date serialization format used during JSON serialization or deserialization.
    /// </summary>
    private readonly string _serializationFormat = serializationFormat ?? "yyyy-MM-dd";

    /// <summary>
    ///     Converts <see cref="DateOnly" /> values to and from JSON using System.Text.Json.
    ///     Supports optional customization of serialization format.
    /// </summary>
    /// <remarks>
    ///     This class ensures proper handling of <see cref="DateOnly" /> during JSON serialization
    ///     and deserialization. By default, the format "yyyy-MM-dd" is used unless specified otherwise.
    /// </remarks>
    public DateOnlyJsonConverter() : this(null)
    {
    }

    /// <summary>
    ///     Reads and converts JSON into a <see cref="DateOnly" /> object using the provided JSON data.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader" /> used to read the JSON data.</param>
    /// <param name="typeToConvert">The type being converted, expected to be <see cref="DateOnly" />.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions" /> that specify serialization options.</param>
    /// <returns>A <see cref="DateOnly" /> object parsed from the JSON data.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="typeToConvert" /> or <paramref name="options" /> is null.
    /// </exception>
    /// <exception cref="FormatException">
    ///     Thrown if the JSON data cannot be converted into a valid <see cref="DateOnly" /> object.
    /// </exception>
    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        ArgumentVerifier.NotNull(typeToConvert, nameof(typeToConvert));
        ArgumentVerifier.NotNull(options, nameof(options));

        string? value = reader.GetString();

        return DateOnly.Parse(value!);
    }

    /// <summary>
    ///     Writes the specified <see cref="DateOnly" /> value to JSON using the given <see cref="Utf8JsonWriter" />.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter" /> to write the value to. This cannot be null.</param>
    /// <param name="value">The <see cref="DateOnly" /> value to be written to JSON.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions" /> used for serialization. This cannot be null.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="options" /> is null.
    /// </exception>
    public override void Write(
        Utf8JsonWriter writer,
        DateOnly value,
        JsonSerializerOptions options
    )
    {
        ArgumentVerifier.NotNull(options, nameof(options));

        writer.WriteStringValue(value.ToString(_serializationFormat));
    }
}