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
using Carcass.DistributedCache.Core.Providers.Abstracts;
using Carcass.DistributedCache.Redis.Options;
using Carcass.DistributedCache.Redis.Providers;
using Carcass.DistributedCache.Redis.Providers.Abstracts;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassRedis(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<RedisOptions, RedisCache>? factory = default,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<RedisOptions>(configuration.GetSection("Carcass:Redis"));

        if (factory is null)
            services.Add(ServiceDescriptor.Describe(
                    typeof(RedisCache),
                    sp =>
                    {
                        IOptions<RedisOptions> optionsAccessor =
                            sp.GetRequiredService<IOptions<RedisOptions>>();

                        return new RedisCache(new RedisCacheOptions
                        {
                            Configuration = optionsAccessor.Value.Configuration
                        }
                        );
                    },
                    lifetime
                )
            );
        else
            services.Add(ServiceDescriptor.Describe(
                    typeof(RedisCache),
                    sp =>
                    {
                        IOptions<RedisOptions> optionsAccessor =
                            sp.GetRequiredService<IOptions<RedisOptions>>();

                        return factory(optionsAccessor.Value);
                    },
                    lifetime
                )
            );

        return services;
    }

    public static IServiceCollection AddCarcassRedisProvider(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(IDistributedCacheProvider),
                typeof(RedisProvider),
                lifetime
            )
        );

        services.Add(ServiceDescriptor.Describe(
                typeof(IRedisProvider),
                typeof(RedisProvider),
                lifetime
            )
        );

        return services;
    }
}