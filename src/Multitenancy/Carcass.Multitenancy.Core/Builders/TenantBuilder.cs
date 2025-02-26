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
using Carcass.Multitenancy.Core.Entities.Abstracts;
using Carcass.Multitenancy.Core.Providers;
using Carcass.Multitenancy.Core.ResolutionStrategies.Abstracts;
using Carcass.Multitenancy.Core.Stores.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable UnusedMember.Global

namespace Carcass.Multitenancy.Core.Builders;

/// <summary>
///     A builder class to configure and register services necessary for multitenancy handling.
/// </summary>
/// <typeparam name="TTenant">The tenant type that implements the <see cref="ITenant" /> interface.</typeparam>
public sealed class TenantBuilder<TTenant> where TTenant : class, ITenant
{
    /// <summary>
    ///     Represents the collection of service descriptors used for dependency injection.
    /// </summary>
    /// <remarks>
    ///     This variable stores the <see cref="IServiceCollection" /> instance, allowing service registrations
    ///     such as adding tenant resolution strategies and tenant stores.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided <see cref="IServiceCollection" /> instance is null during initialization.
    /// </exception>
    private readonly IServiceCollection _services;

    /// <summary>
    ///     Builds and configures multitenancy components for a given tenant type.
    /// </summary>
    /// <typeparam name="TTenant">The type of tenant, implementing the <see cref="ITenant" /> interface.</typeparam>
    public TenantBuilder(IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.AddTransient<TenantProvider<TTenant>>();
        _services = services;
    }

    /// <summary>
    ///     Configures the TenantBuilder to use a specific tenant resolution strategy.
    /// </summary>
    /// <typeparam name="TTenantResolutionStrategy">
    ///     The type of the tenant resolution strategy to be used, implementing <see cref="ITenantResolutionStrategy" />.
    /// </typeparam>
    /// <param name="lifetime">
    ///     The lifetime of the tenant resolution strategy. Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>
    ///     The updated <see cref="TenantBuilder{TTenant}" /> for chaining additional configuration.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <typeparamref name="TTenantResolutionStrategy" /> is null.
    /// </exception>
    public TenantBuilder<TTenant> WithTenantResolutionStrategy<TTenantResolutionStrategy>(
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    ) where TTenantResolutionStrategy : ITenantResolutionStrategy
    {
        _services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        _services.Add(ServiceDescriptor.Describe(
                typeof(ITenantResolutionStrategy),
                typeof(TTenantResolutionStrategy),
                lifetime
            )
        );

        return this;
    }

    /// <summary>
    ///     Registers a specific tenant store implementation for multitenancy.
    /// </summary>
    /// <typeparam name="TTenantStore">The concrete implementation of the ITenantStore interface to register.</typeparam>
    /// <param name="lifetime">
    ///     The lifetime of the registered tenant store service. Defaults to <see cref="ServiceLifetime.Singleton" />.
    /// </param>
    /// <returns>
    ///     Returns the current instance of <see cref="TenantBuilder{TTenant}" />
    ///     to allow chaining additional method calls.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Throws if the service collection is null during the method invocation.
    /// </exception>
    public TenantBuilder<TTenant> WithTenantStore<TTenantStore>(
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    ) where TTenantStore : ITenantStore<TTenant>
    {
        _services.Add(ServiceDescriptor.Describe(
                typeof(ITenantStore<TTenant>),
                typeof(TTenantStore),
                lifetime
            )
        );

        return this;
    }
}