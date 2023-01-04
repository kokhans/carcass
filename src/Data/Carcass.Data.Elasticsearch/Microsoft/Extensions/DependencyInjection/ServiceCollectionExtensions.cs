// MIT License
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
using Carcass.Data.Core.Commands.Notifications.Stores.Abstracts;
using Carcass.Data.Elasticsearch.Conductors;
using Carcass.Data.Elasticsearch.Conductors.Abstracts;
using Carcass.Data.Elasticsearch.Notifications.Stores;
using Carcass.Data.Elasticsearch.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nest;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassElasticsearchConductor(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<ElasticsearchOptions, IElasticClient>? factory = default,
        bool reloadOptions = false
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<ElasticsearchOptions>(configuration.GetSection("Carcass:Elasticsearch"));

        if (reloadOptions)
            services.AddSingleton<IElasticsearchConductor>(sp => new ElasticsearchConductor(
                    sp.GetRequiredService<IOptionsMonitor<ElasticsearchOptions>>(),
                    factory
                )
            );
        else
            services.AddSingleton<IElasticsearchConductor>(sp => new ElasticsearchConductor(
                    sp.GetRequiredService<IOptions<ElasticsearchOptions>>(),
                    factory
                )
            );

        return services;
    }

    public static IServiceCollection AddCarcassElasticsearchNotificationStore(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        return services.AddSingleton<INotificationStore, ElasticsearchNotificationStore>();
    }
}