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
using Carcass.Data.Core.EventSourcing.Aggregates.Repositories.Abstracts;
using Carcass.Data.EventStoreDb.Aggregates.Repositories;
using Carcass.Data.EventStoreDb.Options;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods to register services and related dependencies
///     for working with EventStoreDb and aggregate repositories in an application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds and configures an EventStoreDB client to the service collection,
    ///     using the specified configuration and optional factory for instantiating the client.
    /// </summary>
    /// <param name="services">The service collection to which the EventStoreDB client will be added.</param>
    /// <param name="configuration">The configuration containing the "Carcass:EventStoreDb" section.</param>
    /// <param name="factory">
    ///     An optional factory function to customize the creation of the EventStoreDB client.
    ///     If null, a default instantiation using the connection string from the configuration will be used.
    /// </param>
    /// <param name="lifetime">The service lifetime for the EventStoreDB client. Defaults to Singleton.</param>
    /// <returns>The updated service collection with the EventStoreDB client configured.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> or <paramref name="configuration" /> is null.
    /// </exception>
    public static IServiceCollection AddCarcassEventStoreDb(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<EventStoreDbOptions, EventStoreClient>? factory = null,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<EventStoreDbOptions>(configuration.GetSection("Carcass:EventStoreDb"));

        if (factory is null)
            services.Add(ServiceDescriptor.Describe(
                    typeof(EventStoreClient),
                    sp =>
                    {
                        IOptions<EventStoreDbOptions> optionsAccessor =
                            sp.GetRequiredService<IOptions<EventStoreDbOptions>>();

                        return new EventStoreClient(EventStoreClientSettings.Create(
                                optionsAccessor.Value.ConnectionString
                            )
                        );
                    },
                    lifetime
                )
            );
        else
            services.Add(ServiceDescriptor.Describe(
                    typeof(EventStoreClient),
                    sp =>
                    {
                        IOptions<EventStoreDbOptions> optionsAccessor =
                            sp.GetRequiredService<IOptions<EventStoreDbOptions>>();

                        return factory(optionsAccessor.Value);
                    },
                    lifetime
                )
            );

        return services;
    }

    /// <summary>
    ///     Registers the EventStoreDb aggregate repository implementation of <see cref="IAggregateRepository" /> into the
    ///     specified service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the repository will be added.</param>
    /// <param name="lifetime">
    ///     The lifetime of the service to be registered. Defaults to
    ///     <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>The same <see cref="IServiceCollection" /> instance so that multiple calls can be chained.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassEventStoreDbAggregateRepository(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(IAggregateRepository),
                typeof(EventStoreDbAggregateRepository),
                lifetime
            )
        );

        return services;
    }
}