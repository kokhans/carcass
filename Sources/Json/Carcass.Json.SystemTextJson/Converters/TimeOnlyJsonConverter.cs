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

using Carcass.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Carcass.Json.SystemTextJson.Converters;

public sealed class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    private readonly string _serializationFormat;

    public TimeOnlyJsonConverter() : this(default)
    {
    }

    public TimeOnlyJsonConverter(string? serializationFormat)
        => _serializationFormat = serializationFormat ?? "HH:mm:ss.fff";

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