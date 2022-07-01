// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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
using Carcass.ObjectStorage.Core.Providers.Abstracts;
using Carcass.ObjectStorage.Minio.Conductors;
using Carcass.ObjectStorage.Minio.Conductors.Abstracts;
using Carcass.ObjectStorage.Minio.Options;
using Carcass.ObjectStorage.Minio.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassMinioConductor(
        this IServiceCollection services,
        IConfiguration configuration,
        Func<MinioOptions, MinioClient>? factory = default,
        bool reloadOptions = false
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<MinioOptions>(configuration.GetSection("Carcass:Minio"));

        if (reloadOptions)
            services.AddSingleton<IMinioConductor>(sp => new MinioConductor(
                    sp.GetRequiredService<IOptionsMonitor<MinioOptions>>(),
                    factory
                )
            );
        else
            services.AddSingleton<IMinioConductor>(sp => new MinioConductor(
                    sp.GetRequiredService<IOptions<MinioOptions>>(),
                    factory
                )
            );

        return services;
    }

    public static IServiceCollection AddCarcassMinioProvider(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IObjectStorageProvider, MinioProvider>();
    }
}