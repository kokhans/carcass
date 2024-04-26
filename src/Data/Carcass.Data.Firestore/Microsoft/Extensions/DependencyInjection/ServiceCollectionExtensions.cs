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
using Carcass.Data.Firestore.Options;
using Carcass.Data.Firestore.Sessions;
using Carcass.Data.Firestore.Sessions.Abstracts;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for registering Firestore services in an IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Configures and adds Firestore services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which Firestore services will be added.</param>
    /// <param name="configuration">The configuration object containing Firestore settings.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="services" /> or
    ///     <paramref name="configuration" /> parameter is null.
    /// </exception>
    public static IServiceCollection AddCarcassFirestore(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(configuration, nameof(configuration));

        services.Configure<FirestoreOptions>(configuration.GetSection("Carcass:Firestore"));

        services.AddSingleton(sp =>
        {
            IOptions<FirestoreOptions> optionsAccessor = sp.GetRequiredService<IOptions<FirestoreOptions>>();
            FirestoreClientBuilder firestoreClientBuilder = new() {JsonCredentials = optionsAccessor.Value.Json};

            return FirestoreDb.Create(optionsAccessor.Value.ProjectId, firestoreClientBuilder.Build());
        });

        return services;
    }

    /// <summary>
    ///     Adds Firestore session services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The service collection to which the Firestore session services will be added.</param>
    /// <param name="lifetime">The lifetime of the Firestore session services. Defaults to Singleton.</param>
    /// <returns>The updated IServiceCollection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassFirestoreSession(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.Add(ServiceDescriptor.Describe(
                typeof(IFirestoreSession),
                typeof(FirestoreSession),
                lifetime
            )
        );

        return services;
    }
}