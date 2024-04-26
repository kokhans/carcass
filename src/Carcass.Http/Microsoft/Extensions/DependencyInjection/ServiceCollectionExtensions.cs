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
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Http.Accessors.UserId;
using Carcass.Http.Accessors.UserId.Abstracts;
using Carcass.Http.Providers.UserId;
using Carcass.Http.Providers.UserId.Abstracts;
using Carcass.Http.Settings;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for registering services related to Carcass MVC Core functionality
///     into the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Registers the HttpUserIdentityProvider services, settings, and related dependencies into the service collection.
    ///     This method configures dependencies required for handling user identity based on HTTP context.
    /// </summary>
    /// <param name="services">The service collection to add the dependencies to.</param>
    /// <param name="settings">The settings required to configure the HttpUserIdentityProvider.</param>
    /// <returns>
    ///     The original service collection with the registered dependencies.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="services" /> or <paramref name="settings" /> arguments are null.
    /// </exception>
    public static IServiceCollection AddCarcassHttpUserIdentityProvider(
        this IServiceCollection services,
        HttpUserIdentityProviderSettings settings
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(settings, nameof(settings));

        return services
            .AddSingleton(settings)
            .AddSingleton<IHttpUserIdentityProviderFactory, HttpUserIdentityProviderFactory>()
            .AddSingleton<IHttpUserIdentityProvider, HttpUserIdentityProvider>();
    }

    /// <summary>
    ///     Registers the HTTP-specific user ID accessor implementations in the service collection.
    ///     The method adds dependencies for retrieving user identity-related data via HTTP contexts.
    /// </summary>
    /// <param name="services">The service collection to which the required services are added. Must not be null.</param>
    /// <returns>The updated service collection with the user ID accessor services registered.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassHttpUserIdAccessor(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services
            .AddSingleton<IUserIdAccessor, HttpUserIdAccessor>()
            .AddSingleton<IHttpUserIdAccessor, HttpUserIdAccessor>();
    }
}