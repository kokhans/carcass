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
using Carcass.Media.Cloudinary.Options;
using Carcass.Media.Cloudinary.Providers;
using Carcass.Media.Cloudinary.Providers.Abstracts;
using Carcass.Media.Core.Providers.Abstracts;
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods to configure services related to Cloudinary media functionality within the dependency
///     injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Configures and registers Cloudinary services for media handling within the application context.
    /// </summary>
    /// <param name="services">
    ///     Represents the <see cref="IServiceCollection" /> to which the Cloudinary services will be added.
    /// </param>
    /// <param name="configuration">
    ///     The application configuration that provides the "Carcass:Cloudinary" configuration section for Cloudinary settings.
    /// </param>
    /// <param name="lifetime">
    ///     Specifies the <see cref="ServiceLifetime" /> of the Cloudinary services. Defaults to
    ///     <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>
    ///     Returns the modified <see cref="IServiceCollection" /> with Cloudinary services added.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="services" /> or <paramref name="configuration" /> is null.
    /// </exception>
    public static IServiceCollection AddCarcassCloudinaryMedia(
        this IServiceCollection services,
        IConfiguration configuration,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<CloudinaryOptions>(configuration.GetSection("Carcass:Cloudinary"));

        services.Add(ServiceDescriptor.Describe(
                typeof(Cloudinary),
                sp =>
                {
                    IOptions<CloudinaryOptions> optionsAccessor =
                        sp.GetRequiredService<IOptions<CloudinaryOptions>>();

                    return new Cloudinary(
                        new Account(
                            optionsAccessor.Value.CloudName,
                            optionsAccessor.Value.ApiKey,
                            optionsAccessor.Value.ApiSecret
                        )
                    );
                },
                lifetime
            )
        );

        return services;
    }

    /// <summary>
    ///     Configures the service collection to use the Cloudinary media provider.
    /// </summary>
    /// <param name="services">
    ///     The service collection to which the Cloudinary media provider will be added.
    /// </param>
    /// <param name="lifetime">
    ///     The lifetime of the registered services. Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>
    ///     The modified service collection with the Cloudinary media provider added.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="services" /> parameter is null.
    /// </exception>
    public static IServiceCollection AddCarcassCloudinaryMediaProvider(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(IMediaProvider),
                typeof(CloudinaryMediaProvider),
                lifetime
            )
        );

        services.Add(ServiceDescriptor.Describe(
                typeof(ICloudinaryMediaProvider),
                typeof(CloudinaryMediaProvider),
                lifetime
            )
        );

        return services;
    }
}