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
///     Provides a custom JSON converter for the <see cref="TimeOnly" /> type using System.Text.Json.
///     Enables serialization and deserialization of <see cref="TimeOnly" /> values with a customizable format.
/// </summary>
public sealed class TimeOnlyJsonConverter(string? serializationFormat) : JsonConverter<TimeOnly>
{
    /// <summary>
    ///     Represents the format string used for serializing and deserializing instances of <see cref="TimeOnly" />.
    /// </summary>
    /// <remarks>
    ///     If not explicitly specified, the default format is "HH:mm:ss.fff".
    ///     This string defines how <see cref="TimeOnly" /> values are converted to and from their JSON representations.
    /// </remarks>
    /// <exception cref="FormatException">
    ///     May occur during serialization or deserialization if the format is invalid or incompatible with the data.
    /// </exception>
    private readonly string _serializationFormat = serializationFormat ?? "HH:mm:ss.fff";

    /// <summary>
    ///     Converts <see cref="TimeOnly" /> values to and from JSON using System.Text.Json.
    ///     Supports optional customization of serialization format.
    /// </summary>
    /// <remarks>
    ///     This class ensures proper handling of <see cref="TimeOnly" /> during JSON serialization
    ///     and deserialization. By default, the default time formatting is used unless a specific format is provided.
    /// </remarks>
    public TimeOnlyJsonConverter() : this(null)
    {
    }

    /// <summary>
    ///     Reads and converts the JSON representation of a <see cref="TimeOnly" /> value.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader" /> to read JSON data from.</param>
    /// <param name="typeToConvert">The type of the object to convert, which should be <see cref="TimeOnly" />.</param>
    /// <param name="options">Serialization options to use for reading.</param>
    /// <returns>The deserialized <see cref="TimeOnly" /> value.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="typeToConvert" /> or <paramref name="options" /> is
    ///     null.
    /// </exception>
    /// <exception cref="FormatException">Thrown if the JSON value cannot be parsed into a valid <see cref="TimeOnly" />.</exception>
    public override TimeOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        ArgumentVerifier.NotNull(typeToConvert, nameof(typeToConvert));
        ArgumentVerifier.NotNull(options, nameof(options));

        string? value = reader.GetString();
        return TimeOnly.Parse(value!);
    }

    /// <summary>
    ///     Writes a <see cref="TimeOnly" /> value to a JSON output stream using the specified serialization format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter" /> used to write the JSON value.</param>
    /// <param name="value">The <see cref="TimeOnly" /> value to be serialized to JSON.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions" /> used for serialization configuration.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="options" /> is null.</exception>
    public override void Write(
        Utf8JsonWriter writer,
        TimeOnly value,
        JsonSerializerOptions options
    )
    {
        ArgumentVerifier.NotNull(options, nameof(options));

        writer.WriteStringValue(value.ToString(_serializationFormat));
    }
}