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
using Carcass.Metadata.Accessors.AdHoc;
using Carcass.Metadata.Accessors.AdHoc.Abstracts;
using Carcass.Metadata.Options;
using Carcass.Metadata.Stores;
using Carcass.Metadata.Stores.Abstracts;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods to add and configure services related to Carcass metadata in the
///     dependency injection container.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    ///     Registers the InMemoryMetadataStore service for use as an implementation of IMetadataStore.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the service will be added.
    /// </param>
    /// <returns>
    ///     The modified <see cref="IServiceCollection" /> that includes the registered InMemoryMetadataStore service.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> argument is null.
    /// </exception>
    public static IServiceCollection AddCarcassInMemoryMetadataStore(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IMetadataStore, InMemoryMetadataStore>();
    }

    /// <summary>
    ///     Adds and configures the ad hoc metadata accessor services to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The service collection to which the ad hoc metadata accessor will be added.</param>
    /// <param name="resolutionStrategy">
    ///     The strategy to resolve ad hoc metadata. Defaults to
    ///     <see cref="AdHocMetadataResolutionStrategy.ReplaceWithAdHoc" />.
    /// </param>
    /// <returns>The modified service collection with the ad hoc metadata accessor configured.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassAdHocMetadataAccessor(
        this IServiceCollection services,
        AdHocMetadataResolutionStrategy resolutionStrategy = AdHocMetadataResolutionStrategy.ReplaceWithAdHoc
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services
            .Configure<AdHocMetadataAccessorOptions>(ahmao => ahmao.ResolutionStrategy = resolutionStrategy)
            .AddScoped<IAdHocMetadataAccessor, AdHocMetadataAccessor>();
    }
}