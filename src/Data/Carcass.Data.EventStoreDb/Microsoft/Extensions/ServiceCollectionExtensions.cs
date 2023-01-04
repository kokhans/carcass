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
using Carcass.Data.Core.Aggregates.Repositories.Abstracts;
using Carcass.Data.EventStoreDb.Aggregates.Repositories;
using Carcass.Data.EventStoreDb.Conductors;
using Carcass.Data.EventStoreDb.Conductors.Abstracts;
using Carcass.Data.EventStoreDb.Options;
using EventStore.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassEventStoreDbConductor(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<EventStoreDbOptions, EventStoreClient>? factory = default,
        bool reloadOptions = false
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<EventStoreDbOptions>(configuration.GetSection("Carcass:EventStoreDb"));

        if (reloadOptions)
            services.AddSingleton<IEventStoreDbConductor>(sp => new EventStoreDbConductor(
                    sp.GetRequiredService<IOptionsMonitor<EventStoreDbOptions>>(),
                    factory
                )
            );
        else
            services.AddSingleton<IEventStoreDbConductor>(sp => new EventStoreDbConductor(
                    sp.GetRequiredService<IOptions<EventStoreDbOptions>>(),
                    factory
                )
            );

        return services;
    }

    public static IServiceCollection AddCarcassEventStoreDbAggregateRepository(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IAggregateRepository, EventStoreDbAggregateRepository>();
    }
}