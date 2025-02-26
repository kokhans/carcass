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
using Carcass.Firebase.Core.Options;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for adding Firebase-related services to the
///     <see cref="IServiceCollection" /> in a .NET dependency injection configuration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Configures Firebase services for the application by setting up necessary Firebase options and initializing the
    ///     Firebase application.
    /// </summary>
    /// <param name="services">The service collection to which Firebase services are added.</param>
    /// <param name="configuration">The application configuration used to retrieve Firebase settings.</param>
    /// <returns>The same service collection to allow method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> or <paramref name="configuration" />
    ///     is null.
    /// </exception>
    public static IServiceCollection AddCarcassFirebase(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<FirebaseOptions>(configuration.GetSection("Carcass:Firebase"));

        services.AddSingleton(sp =>
        {
            IOptions<FirebaseOptions> optionsAccessor = sp.GetRequiredService<IOptions<FirebaseOptions>>();

            return FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(optionsAccessor.Value.Json)
            });
        });

        return services;
    }
}