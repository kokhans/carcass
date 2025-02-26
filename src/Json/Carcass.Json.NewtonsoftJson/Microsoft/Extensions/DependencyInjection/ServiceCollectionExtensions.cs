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

using Carcass.Core;
using Carcass.Json.Core.Providers.Abstracts;
using Carcass.Json.NewtonsoftJson.Providers;
using Carcass.Json.NewtonsoftJson.Providers.Abstracts;
using Carcass.Json.NewtonsoftJson.Settings;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for adding services related to Newtonsoft.Json to an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a Newtonsoft JSON provider to the service collection for JSON serialization and deserialization.
    /// </summary>
    /// <param name="services">
    ///     The service collection to which the JSON provider will be added. This parameter cannot be null.
    /// </param>
    /// <param name="settings">
    ///     Optional custom JSON serializer settings for configuring the Newtonsoft JSON provider. If null, default settings
    ///     will be used.
    /// </param>
    /// <returns>
    ///     The modified service collection with the JSON provider registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> parameter is null.
    /// </exception>
    public static IServiceCollection AddCarcassNewtonsoftJsonProvider(
        this IServiceCollection services,
        JsonSerializerSettings? settings = null
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        NewtonsoftJsonProvider newtonsoftJsonProvider = new(
            settings ?? NewtonsoftJsonSettings.Defaults()
        );

        return services
            .AddSingleton<IJsonProvider>(newtonsoftJsonProvider)
            .AddSingleton<INewtonsoftJsonProvider>(newtonsoftJsonProvider);
    }
}