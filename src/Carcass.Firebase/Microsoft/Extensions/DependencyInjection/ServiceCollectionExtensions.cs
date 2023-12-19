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
using Carcass.Core.Accessors.TenantId.Abstracts;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Firebase.Accessors;
using Carcass.Firebase.Accessors.Abstracts;
using Carcass.Firebase.AuthenticationHandlers;
using Carcass.Firebase.Options;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
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

    public static IServiceCollection AddCarcassFirebaseAuthenticationHandler(
        this IServiceCollection services,
        Action<JwtBearerOptions>? configure = default
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<JwtBearerOptions, FirebaseAuthenticationHandler>(
                JwtBearerDefaults.AuthenticationScheme,
                aso => configure?.Invoke(aso)
            );

        return services;
    }

    public static IServiceCollection AddCarcassFirebaseUserAccessor(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services
            .AddSingleton<IUserIdAccessor, FirebaseUserAccessor>()
            .AddSingleton<ITenantIdAccessor, FirebaseUserAccessor>()
            .AddSingleton<IFirebaseUserAccessor, FirebaseUserAccessor>();

        return services;
    }
}