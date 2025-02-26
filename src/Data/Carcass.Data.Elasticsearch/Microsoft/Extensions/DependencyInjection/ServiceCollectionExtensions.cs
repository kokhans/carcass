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
using Carcass.Data.Core.Audit.Abstracts;
using Carcass.Data.Elasticsearch.Audit;
using Carcass.Data.Elasticsearch.Options;
using Elastic.Clients.Elasticsearch;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for IServiceCollection to register services related to Carcass Elasticsearch
///     functionality.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds and configures the Elasticsearch client to the provided service collection.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the Elasticsearch client will be added.
    /// </param>
    /// <param name="configuration">
    ///     The <see cref="IConfiguration" /> instance used to retrieve Elasticsearch configuration options.
    /// </param>
    /// <param name="factory">
    ///     An optional delegate for creating a custom <see cref="ElasticsearchClient" /> instance.
    ///     If null, a default client is created using the configuration.
    /// </param>
    /// <param name="lifetime">
    ///     Specifies the lifetime of the registered <see cref="ElasticsearchClient" /> instance. Defaults to Singleton.
    /// </param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> after the Elasticsearch client has been registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> or <paramref name="configuration" /> parameters are null.
    /// </exception>
    public static IServiceCollection AddCarcassElasticsearch(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<ElasticsearchOptions, ElasticsearchClient>? factory = null,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<ElasticsearchOptions>(configuration.GetSection("Carcass:Elasticsearch"));

        if (factory is null)
            services.Add(ServiceDescriptor.Describe(
                    typeof(ElasticsearchClient),
                    sp =>
                    {
                        IOptions<ElasticsearchOptions> optionsAccessor =
                            sp.GetRequiredService<IOptions<ElasticsearchOptions>>();

                        return new ElasticsearchClient(optionsAccessor.Value.Uri);
                    },
                    lifetime
                )
            );
        else
            services.Add(ServiceDescriptor.Describe(
                    typeof(ElasticsearchClient),
                    sp =>
                    {
                        IOptions<ElasticsearchOptions> optionsAccessor =
                            sp.GetRequiredService<IOptions<ElasticsearchOptions>>();

                        return factory(optionsAccessor.Value);
                    },
                    lifetime
                )
            );

        return services;
    }

    /// <summary>
    ///     Registers the Elasticsearch audit entry notification handler.
    ///     This handler processes audit entry notifications and integrates them with Elasticsearch.
    /// </summary>
    /// <param name="services">
    ///     The service collection to which the Elasticsearch audit entry notification handler will be added.
    /// </param>
    /// <param name="configuration">
    ///     The application's configuration used to configure the Elasticsearch-related settings.
    /// </param>
    /// <returns>
    ///     Returns the updated service collection for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the services or configuration parameter is null.
    /// </exception>
    public static IServiceCollection AddCarcassElasticsearchAuditEntryNotificationHandler(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        return services
            .AddTransient<INotificationHandler<IAuditEntryNotification>, ElasticsearchAuditEntryNotificationHandler>();
    }
}