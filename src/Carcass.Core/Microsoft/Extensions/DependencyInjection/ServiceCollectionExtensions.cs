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
using Carcass.Core.Accessors.CorrelationId;
using Carcass.Core.Accessors.CorrelationId.Abstracts;
using Carcass.Core.Accessors.TenantId;
using Carcass.Core.Accessors.TenantId.Abstracts;
using Carcass.Core.Accessors.UserId;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Core.Dependencies;
using Carcass.Core.Locators;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for the IServiceCollection interface
///     to register and configure Carcass-related dependencies and accessors.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds a <see cref="DependencyStore{TDependency}" /> to the service collection,
    ///     allowing for the management and registration of dependencies.
    /// </summary>
    /// <typeparam name="TDependency">The type of the dependency to be stored.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the dependency store to.</param>
    /// <param name="dependencyStore">The <see cref="DependencyStore{TDependency}" /> instance to be added.</param>
    /// <param name="dependencyResolverRegistrar">
    ///     An optional action to further configure or register additional dependencies within the service collection.
    /// </param>
    /// <returns>The updated <see cref="IServiceCollection" /> containing the added dependency store.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="services" /> or <paramref name="dependencyStore" /> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddCarcassDependencyStore<TDependency>(
        this IServiceCollection services,
        DependencyStore<TDependency> dependencyStore,
        Action<IServiceCollection>? dependencyResolverRegistrar = null
    ) where TDependency : class
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(dependencyStore, nameof(dependencyStore));

        services.AddSingleton(dependencyStore);
        dependencyResolverRegistrar?.Invoke(services);

        return services;
    }

    /// <summary>
    ///     Adds a dependency resolver to the service collection for resolving dependencies of the specified type.
    /// </summary>
    /// <typeparam name="TDependency">The type of the dependency to resolve.</typeparam>
    /// <param name="services">The service collection to add the dependency resolver to.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services" /> argument is null.</exception>
    public static void AddCarcassDependencyResolver<TDependency>(this IServiceCollection services)
        where TDependency : class
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.AddSingleton<DependencyResolver<TDependency>>(sp => name =>
            sp.GetRequiredService<DependencyStore<TDependency>>().GetRequiredDependency(name)
        );
    }

    /// <summary>
    ///     Adds a default implementation of <see cref="IUserIdAccessor" /> that provides nullable user ID functionality
    ///     to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the nullable user ID accessor will be added.</param>
    /// <returns>The updated service collection with the nullable user ID accessor registered.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services" /> argument is null.</exception>
    public static IServiceCollection AddCarcassNullableUserIdAccessor(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IUserIdAccessor, NullableUserIdAccessor>();
    }

    /// <summary>
    ///     Adds an implementation of <see cref="ICorrelationIdAccessor" /> to the service collection that allows nullable
    ///     correlation ID access within the application.
    /// </summary>
    /// <param name="services">The service collection to which the implementation will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection" /> with the registered nullable correlation ID accessor.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> argument is null.</exception>
    public static IServiceCollection AddCarcassNullableCorrelationIdAccessor(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<ICorrelationIdAccessor, NullableCorrelationIdAccessor>();
    }

    /// <summary>
    ///     Registers an implementation of <see cref="ITenantIdAccessor" /> as a singleton service in the dependency injection
    ///     container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the service to.</param>
    /// <returns>The updated <see cref="IServiceCollection" /> to allow for fluent chaining of service registration methods.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassNullableTenantIdAccessor(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<ITenantIdAccessor, NullableTenantIdAccessor>();
    }

    /// <summary>
    ///     Registers a global service provider instance to enable service location functionality.
    /// </summary>
    /// <param name="services">The service collection to use for building the service provider. It must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static void AddCarcassServiceProviderLocator(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        ServiceProviderLocator.Set(services.BuildServiceProvider());
    }
}