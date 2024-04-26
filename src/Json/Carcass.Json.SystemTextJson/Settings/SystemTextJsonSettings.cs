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
using Carcass.Json.SystemTextJson.Converters;

// ReSharper disable ClassNeverInstantiated.Global

namespace Carcass.Json.SystemTextJson.Settings;

/// <summary>
///     Provides default configuration settings for System.Text.Json serialization and deserialization.
/// </summary>
public sealed class SystemTextJsonSettings
{
    /// <summary>
    ///     Provides default JSON serialization settings for System.Text.Json.
    /// </summary>
    /// <returns>
    ///     A configured instance of <see cref="JsonSerializerOptions" /> with custom converters
    ///     and case-insensitive property name handling.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if there is an issue initializing the <see cref="JsonSerializerOptions" /> or converters.
    /// </exception>
    public static JsonSerializerOptions Defaults()
    {
        JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        jsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        jsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());

        return jsonSerializerOptions;
    }
}