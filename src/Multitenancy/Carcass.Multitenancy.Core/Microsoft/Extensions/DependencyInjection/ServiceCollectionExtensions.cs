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
using Carcass.Multitenancy.Core.Builders;
using Carcass.Multitenancy.Core.Entities.Abstracts;
using Carcass.Multitenancy.Core.Providers;
using Carcass.Multitenancy.Core.Providers.Abstracts;
using Carcass.Multitenancy.Core.Stores;
using Carcass.Multitenancy.Core.Stores.Abstracts;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods to configure and manage multitenancy-related services in an application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds an in-memory tenant store implementation to the service collection for managing tenant data.
    /// </summary>
    /// <typeparam name="TTenant">The type of tenant entity, which must implement the <see cref="ITenant" /> interface.</typeparam>
    /// <param name="services">The service collection to which the in-memory tenant store will be added.</param>
    /// <returns>The updated service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddCarcassInMemoryTenantStore<TTenant>(
        this IServiceCollection services
    ) where TTenant : class, ITenant
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<ITenantStore<TTenant>, InMemoryTenantStore<TTenant>>();
    }

    /// <summary>
    ///     Adds a tenant provider singleton service to the service collection for multitenancy support.
    /// </summary>
    /// <typeparam name="TTenant">
    ///     The type of the tenant that implements <see cref="ITenant" />.
    /// </typeparam>
    /// <param name="services">
    ///     The service collection to which the tenant provider is added.
    /// </param>
    /// <returns>
    ///     The updated service collection.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> parameter is null.
    /// </exception>
    public static IServiceCollection AddCarcassTenantProvider<TTenant>(
        this IServiceCollection services
    ) where TTenant : class, ITenant
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<ITenantProvider<TTenant>, TenantProvider<TTenant>>();
    }

    /// <summary>
    ///     Configures and registers multitenancy support for the specified tenant type.
    /// </summary>
    /// <typeparam name="TTenant">The tenant type that implements the <see cref="ITenant" /> interface.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add multitenancy services to.</param>
    /// <returns>An instance of <see cref="TenantBuilder{TTenant}" /> for further configuration.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services" /> is null.</exception>
    public static TenantBuilder<TTenant> AddCarcassMultitenancy<TTenant>(
        this IServiceCollection services
    ) where TTenant : class, ITenant
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return new TenantBuilder<TTenant>(services);
    }
}