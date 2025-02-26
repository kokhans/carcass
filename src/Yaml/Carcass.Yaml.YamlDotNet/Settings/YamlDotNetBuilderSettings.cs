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

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// ReSharper disable ClassNeverInstantiated.Global

namespace Carcass.Yaml.YamlDotNet.Settings;

/// <summary>
///     Provides configuration methods to initialize default settings for YamlDotNet's SerializerBuilder and
///     DeserializerBuilder.
/// </summary>
public sealed class YamlDotNetBuilderSettings
{
    /// <summary>
    ///     Creates a default instance of the <see cref="SerializerBuilder" /> configured with the UnderscoredNamingConvention.
    /// </summary>
    /// <returns>A <see cref="SerializerBuilder" /> instance configured with the UnderscoredNamingConvention.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the SerializerBuilder configuration process fails.</exception>
    public static SerializerBuilder SerializerDefaults() => new SerializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance);

    /// <summary>
    ///     Creates a default instance of the <see cref="DeserializerBuilder" /> configured with the
    ///     UnderscoredNamingConvention.
    /// </summary>
    /// <returns>A <see cref="DeserializerBuilder" /> instance configured with the UnderscoredNamingConvention.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the DeserializerBuilder configuration process fails.</exception>
    public static DeserializerBuilder DeserializerDefaults() => new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance);
}