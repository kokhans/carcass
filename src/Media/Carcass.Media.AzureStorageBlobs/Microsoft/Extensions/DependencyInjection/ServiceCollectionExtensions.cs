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

using Azure.Storage.Blobs;
using Carcass.Core;
using Carcass.Media.AzureStorageBlobs.Providers;
using Carcass.Media.AzureStorageBlobs.Providers.Abstracts;
using Carcass.Media.Core.Providers.Abstracts;
using Microsoft.Extensions.Configuration;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for configuring Carcass Azure Storage Blobs media-related services
///     within an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the Azure Storage Blob service to the specified service collection, allowing the service to
    ///     interact with Azure Storage using the provided configuration settings.
    /// </summary>
    /// <param name="services">The service collection where the Blob service client will be registered.</param>
    /// <param name="configuration">The configuration instance containing the Azure Storage connection string.</param>
    /// <param name="lifetime">The lifetime of the service registered (Singleton by default).</param>
    /// <returns>
    ///     The updated <see cref="IServiceCollection" /> instance with the Azure Storage Blob service registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if either the <paramref name="services" /> or <paramref name="configuration" /> is null.
    /// </exception>
    public static IServiceCollection AddCarcassAzureStorageBlobsMedia(
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Add(ServiceDescriptor.Describe(
                typeof(BlobServiceClient),
                _ => new BlobServiceClient(configuration.GetConnectionString("AzureStorage")),
                lifetime
            )
        );

        return services;
    }

    /// <summary>
    ///     Registers the Azure Storage Blobs media provider services into the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the services are added.</param>
    /// <param name="lifetime">
    ///     The lifetime of the registered services. The default value is
    ///     <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection" /> to support chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> argument is null.</exception>
    public static IServiceCollection AddCarcassAzureStorageBlobsMediaProvider(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(IMediaProvider),
                typeof(AzureStorageBlobsMediaProvider),
                lifetime
            )
        );

        services.Add(ServiceDescriptor.Describe(
                typeof(IAzureStorageBlobsMediaProvider),
                typeof(AzureStorageBlobsMediaProvider),
                lifetime
            )
        );

        return services;
    }
}