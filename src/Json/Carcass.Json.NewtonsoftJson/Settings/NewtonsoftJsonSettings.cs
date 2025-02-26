﻿// MIT License
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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global

namespace Carcass.Json.NewtonsoftJson.Settings;

/// <summary>
///     Provides a collection of default configuration settings for Newtonsoft.Json serialization and deserialization.
/// </summary>
public sealed class NewtonsoftJsonSettings
{
    /// <summary>
    ///     Provides default settings for JSON serialization using Newtonsoft.Json.
    /// </summary>
    /// <returns>
    ///     A configured instance of <see cref="JsonSerializerSettings" /> that includes settings
    ///     such as ignoring null values, handling reference loops, and using camel case
    ///     property naming.
    /// </returns>
    /// <exception cref="JsonException">
    ///     Thrown if the default settings could not be constructed.
    /// </exception>
    public static JsonSerializerSettings Defaults() => new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        ContractResolver = new CamelCasePropertyNamesContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        Converters = new List<JsonConverter>
        {
            new StringEnumConverter()
        },
        TypeNameHandling = TypeNameHandling.Auto
    };
}