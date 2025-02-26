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

using Carcass.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMember.Global

namespace Carcass.Core.Locators;

/// <summary>
///     Provides a centralized mechanism for locating services available within a dependency injection container.
/// </summary>
public sealed class ServiceProviderLocator
{
    /// <summary>
    ///     Provides access to the current instance of <see cref="ServiceProviderLocator" />.
    ///     This allows resolving services using a static context.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to access the current instance before it has been initialized
    ///     with a valid <see cref="ServiceProvider" />.
    /// </exception>
    private static ServiceProviderLocator? _current;

    /// <summary>
    ///     Represents an instance of the <see cref="ServiceProvider" /> used to resolve
    ///     service dependencies within the application.
    /// </summary>
    /// <remarks>
    ///     This field is set during the initialization of the <see cref="ServiceProviderLocator" />
    ///     and is utilized to retrieve services from the IoC container.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="ServiceProvider" /> is not properly initialized or
    ///     if required services are not registered.
    /// </exception>
    private readonly ServiceProvider _serviceProvider;

    /// <summary>
    ///     A locator class for resolving services from the underlying <see cref="ServiceProvider" />.
    ///     Acts as a mechanism to access services in cases where direct dependency injection is not feasible.
    /// </summary>
    /// <remarks>
    ///     This class is designed to provide service resolution functionalities and is meant to be used sparingly.
    ///     Direct dependency injection should be preferred whenever possible.
    /// </remarks>
    private ServiceProviderLocator(ServiceProvider serviceProvider)
    {
        ArgumentVerifier.NotNull(serviceProvider, nameof(serviceProvider));

        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Gets the current instance of <see cref="ServiceProviderLocator" />.
    /// </summary>
    /// <returns>The current instance of <see cref="ServiceProviderLocator" />.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the current instance is not set.</exception>
    public static ServiceProviderLocator Current
    {
        get
        {
            if (_current is null)
                throw new InvalidOperationException($"{nameof(ServiceProvider)} is not set.");

            return _current;
        }
    }

    /// <summary>
    ///     Retrieves an optional service of the specified type from the service provider.
    /// </summary>
    /// <param name="serviceType">The type of the service to retrieve.</param>
    /// <returns>
    ///     An instance of the specified service type if available; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="serviceType" /> is null.
    /// </exception>
    public object? GetOptionalService(Type serviceType)
    {
        ArgumentVerifier.NotNull(serviceType, nameof(serviceType));

        return _serviceProvider.GetService(serviceType);
    }

    /// <summary>
    ///     Retrieves an optional service of the specified type from the underlying service provider.
    ///     Returns null if the service is not registered or cannot be resolved.
    /// </summary>
    /// <typeparam name="TService">The type of the service to retrieve.</typeparam>
    /// <returns>The resolved service instance of type <typeparamref name="TService" />, or null if not found.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the service provider is not properly configured or the service cannot be instantiated.
    /// </exception>
    public TService? GetOptionalService<TService>() => _serviceProvider.GetService<TService>();

    /// <summary>
    ///     Retrieves a service object of the specified type from the service provider.
    ///     This method ensures the requested service is available and throws an exception if it is not.
    /// </summary>
    /// <param name="serviceType">The type of the service to retrieve.</param>
    /// <returns>An object representing the requested service.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="serviceType" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the requested service is not registered or cannot be resolved.</exception>
    public object GetRequiredService(Type serviceType)
    {
        ArgumentVerifier.NotNull(serviceType, nameof(serviceType));

        return _serviceProvider.GetRequiredService(serviceType);
    }

    /// <summary>
    ///     Retrieves a required service of the specified type from the service provider.
    /// </summary>
    /// <typeparam name="TService">The type of the service to retrieve. Must be non-nullable.</typeparam>
    /// <returns>The service instance of the specified type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when no service of the specified type is registered in the service provider.
    /// </exception>
    public TService GetRequiredService<TService>() where TService : notnull
    {
        TService? service = _serviceProvider.GetService<TService>();
        if (service is not null)
            return service;

        throw new InvalidOperationException(
            $"No required service of type {typeof(TService).Name} is registered."
        );
    }

    /// <summary>
    ///     Retrieves an optional collection of services of type <typeparamref name="TService" /> from the current service
    ///     provider.
    /// </summary>
    /// <typeparam name="TService">The type of services to retrieve.</typeparam>
    /// <returns>
    ///     A read-only collection of services of type <typeparamref name="TService" /> if any are registered; otherwise, null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the current service provider is not properly initialized or set.
    /// </exception>
    public IReadOnlyCollection<TService>? GetOptionalServices<TService>()
    {
        IList<TService> services = _serviceProvider.GetServices<TService>().ToList();

        return services.Any() ? services.AsReadOnlyCollection() : null;
    }

    /// <summary>
    ///     Retrieves all required services of a specified type from the service provider.
    /// </summary>
    /// <typeparam name="TService">The type of the service to retrieve. This type must not be null.</typeparam>
    /// <returns>A read-only collection containing all registered instances of the specified service type.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if no required services of the specified type are registered in the service provider.
    /// </exception>
    public IReadOnlyCollection<TService> GetRequiredServices<TService>() where TService : notnull
    {
        IList<TService> services = _serviceProvider.GetServices<TService>().ToList();
        if (services.Any())
            return services.AsReadOnlyCollection();

        throw new InvalidOperationException(
            $"No required services of type {typeof(TService).Name} are registered."
        );
    }

    /// <summary>
    ///     Initializes the static service provider locator with the provided <see cref="ServiceProvider" /> instance.
    /// </summary>
    /// <param name="serviceProvider">
    ///     The <see cref="ServiceProvider" /> used to initialize the service provider locator.
    ///     Cannot be null.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider" /> is null.</exception>
    public static void Set(ServiceProvider serviceProvider)
    {
        ArgumentVerifier.NotNull(serviceProvider, nameof(serviceProvider));

        _current = new ServiceProviderLocator(serviceProvider);
    }
}