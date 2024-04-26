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

using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Carcass.Core.Extensions;

/// <summary>
///     Provides extension methods for modifying an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Removes a service descriptor of the specified type <typeparamref name="T" /> from the service collection.
    /// </summary>
    /// <typeparam name="T">The type of service to be removed.</typeparam>
    /// <param name="services">The service collection from which the service descriptor will be removed.</param>
    /// <returns>The updated <see cref="IServiceCollection" /> after the removal of the specified service descriptor.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services" /> parameter is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if a service descriptor for the specified type <typeparamref name="T" /> is not found in the collection.
    /// </exception>
    public static IServiceCollection RemoveServiceDescriptor<T>(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        ServiceDescriptor? serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(T));
        if (serviceDescriptor is not null)
            services.Remove(serviceDescriptor);
        else
            throw new InvalidOperationException($"{nameof(ServiceDescriptor)} {nameof(T)} not found.");

        return services;
    }

    /// <summary>
    ///     Attempts to remove the service descriptor of the specified type from the service collection if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the service descriptor to be removed.</typeparam>
    /// <param name="services">The service collection from which the service descriptor will be removed.</param>
    /// <returns>The modified service collection, with the target service descriptor removed if it was present.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="services" /> is null.</exception>
    public static IServiceCollection TryRemoveServiceDescriptor<T>(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        ServiceDescriptor? serviceDescriptor = services.FirstOrDefault(sd => sd.ServiceType == typeof(T));
        if (serviceDescriptor is not null)
            services.Remove(serviceDescriptor);

        return services;
    }
}