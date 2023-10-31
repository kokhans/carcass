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
using Carcass.Data.Firestore.Options;
using Carcass.Data.Firestore.Sessions;
using Carcass.Data.Firestore.Sessions.Abstracts;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
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
            FirestoreClientBuilder firestoreClientBuilder = new() { JsonCredentials = optionsAccessor.Value.Json };

            return FirestoreDb.Create(optionsAccessor.Value.ProjectId, firestoreClientBuilder.Build());
        });

        return services;
    }

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