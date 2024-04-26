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
using Carcass.Swashbuckle.Exclude;
using Carcass.Swashbuckle.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for registering Swashbuckle configurations and integrations in an application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Configures and registers Swashbuckle services to the application's service collection.
    /// </summary>
    /// <param name="services">
    ///     The IServiceCollection to which Swashbuckle services are added.
    /// </param>
    /// <param name="configuration">
    ///     The IConfiguration used to retrieve Swashbuckle options from the "Carcass:Swashbuckle" section.
    /// </param>
    /// <param name="configure">
    ///     An optional Action to customize the SwaggerGenOptions.
    /// </param>
    /// <returns>
    ///     The IServiceCollection with Swashbuckle services added.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when either <paramref name="services" /> or <paramref name="configuration" /> is null.
    /// </exception>
    public static IServiceCollection AddCarcassSwashbuckle(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<SwaggerGenOptions>? configure = null
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<SwashbuckleOptions>(configuration.GetSection("Carcass:Swashbuckle"));

        ServiceProvider serviceProvider = services.BuildServiceProvider(true);

        IOptions<SwashbuckleOptions> optionsAccessor =
            serviceProvider.GetRequiredService<IOptions<SwashbuckleOptions>>();

        return services
            .AddSwaggerGen(sgo =>
                {
                    sgo.SchemaFilter<SwashbuckleExcludeSchemaFilter>();
                    sgo.SwaggerDoc(
                        optionsAccessor.Value.Version,
                        new OpenApiInfo
                        {
                            Title = optionsAccessor.Value.Name,
                            Version = optionsAccessor.Value.Version
                        }
                    );
                    sgo.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
                    configure?.Invoke(sgo);
                }
            ).AddSwaggerGenNewtonsoftSupport();
    }
}