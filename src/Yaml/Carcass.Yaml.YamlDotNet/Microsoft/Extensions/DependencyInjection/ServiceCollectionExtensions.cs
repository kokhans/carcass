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

using Carcass.Core;
using Carcass.Yaml.Core.Providers.Abstracts;
using Carcass.Yaml.YamlDotNet.Providers;
using Carcass.Yaml.YamlDotNet.Providers.Abstracts;
using Carcass.Yaml.YamlDotNet.Settings;
using YamlDotNet.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for configuring YAML-related services in an IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the Carcass YAML DotNet provider to the specified service collection.
    /// </summary>
    /// <param name="services">The service collection to which the YAML provider will be added.</param>
    /// <param name="serializerBuilder">
    ///     An optional serializer builder instance for customized serialization settings. If null, default settings will be
    ///     used.
    /// </param>
    /// <param name="deserializerBuilder">
    ///     An optional deserializer builder instance for customized deserialization settings. If null, default settings will
    ///     be used.
    /// </param>
    /// <returns>The modified service collection with the YAML provider registered.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassYamlDotNetProvider(
        this IServiceCollection services,
        SerializerBuilder? serializerBuilder = null,
        DeserializerBuilder? deserializerBuilder = null
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        YamlDotNetProvider yamlDotNetProvider = new(
            serializerBuilder is null
                ? YamlDotNetBuilderSettings.SerializerDefaults().Build()
                : serializerBuilder.Build(),
            deserializerBuilder is null
                ? YamlDotNetBuilderSettings.DeserializerDefaults().Build()
                : deserializerBuilder.Build()
        );

        return services
            .AddSingleton<IYamlProvider>(yamlDotNetProvider)
            .AddSingleton<IYamlDotNetProvider>(yamlDotNetProvider);
    }
}