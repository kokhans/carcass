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
using Carcass.Core.Accessors.TenantId.Abstracts;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Firebase.AspNetCore.Accessors;
using Carcass.Firebase.AspNetCore.AuthenticationHandlers;
using Carcass.Firebase.Core.Accessors.Abstracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for configuring Firebase authentication
///     and user accessor services in an ASP.NET Core application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the Firebase authentication handler to the service collection for ASP.NET Core applications.
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to which the authentication handler is added.
    /// </param>
    /// <param name="configure">
    ///     An optional action to configure the <see cref="JwtBearerOptions" /> used by the Firebase authentication handler.
    /// </param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> to allow method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> parameter is null.
    /// </exception>
    public static IServiceCollection AddCarcassFirebaseAspNetCoreAuthenticationHandler(
        this IServiceCollection services,
        Action<JwtBearerOptions>? configure = null
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

    /// <summary>
    ///     Adds the Firebase ASP.NET Core User Accessor to the dependency injection container.
    /// </summary>
    /// <param name="services">Instance of <see cref="IServiceCollection" /> to which the accessor will be added.</param>
    /// <returns>The modified <see cref="IServiceCollection" /> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassFirebaseAspNetCoreUserAccessor(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services
            .AddSingleton<IUserIdAccessor, FirebaseAspNetCoreUserAccessor>()
            .AddSingleton<ITenantIdAccessor, FirebaseAspNetCoreUserAccessor>()
            .AddSingleton<IFirebaseUserAccessor, FirebaseAspNetCoreUserAccessor>();

        return services;
    }
}