﻿// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nest;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassElasticsearch(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<ElasticsearchOptions, IElasticClient>? factory = default,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<ElasticsearchOptions>(configuration.GetSection("Carcass:Elasticsearch"));

        if (factory is null)
            services.Add(ServiceDescriptor.Describe(
                    typeof(IElasticClient),
                    sp =>
                    {
                        IOptions<ElasticsearchOptions> optionsAccessor =
                            sp.GetRequiredService<IOptions<ElasticsearchOptions>>();

                        return new ElasticClient(optionsAccessor.Value.Uri);
                    },
                    lifetime
                )
            );
        else
            services.Add(ServiceDescriptor.Describe(
                    typeof(IElasticClient),
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