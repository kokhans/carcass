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
using Carcass.SignalR.Dispatchers;
using Carcass.SignalR.Dispatchers.Abstracts;
using Carcass.SignalR.Publishers.Abstracts;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Provides extension methods for registering services related to SignalR publishers
///     and message dispatchers in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtension
{
    /// <summary>
    ///     Registers a SignalR publisher of type <typeparamref name="TPublisher" /> in the service collection.
    /// </summary>
    /// <typeparam name="TPublisher">
    ///     The specific type of the SignalR publisher to be added. This type must inherit from
    ///     <see cref="HubPublisher{THub,TMessage}" />.
    /// </typeparam>
    /// <param name="services">
    ///     The service collection to which the SignalR publisher will be added.
    /// </param>
    /// <returns>
    ///     The updated service collection with the SignalR publisher registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="services" /> parameter is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if the type <typeparamref name="TPublisher" /> does not inherit from
    ///     <see cref="HubPublisher{THub, TMessage}" />.
    /// </exception>
    public static IServiceCollection AddCarcassSignalRPublisher<TPublisher>(
        this IServiceCollection services
    ) where TPublisher : class
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        Type publisherType = typeof(TPublisher);
        Type hubPublisherType = typeof(HubPublisher<,>);
        if (publisherType.BaseType is not {IsGenericType: true} ||
            publisherType.BaseType.GetGenericTypeDefinition() != hubPublisherType)
            throw new ArgumentException(
                $"{publisherType.FullName} should implement {hubPublisherType.FullName}.");

        services.AddSingleton(publisherType.BaseType, publisherType);
        services.AddSingleton<TPublisher>();

        return services;
    }

    /// <summary>
    ///     Registers the InMemoryMessageDispatcher as the implementation of IMessageDispatcher in the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection instance used to register the service.</param>
    /// <returns>The updated IServiceCollection instance to allow for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="services" /> parameter is null.</exception>
    public static IServiceCollection AddCarcassInMemoryMessageDispatcher(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IMessageDispatcher, InMemoryMessageDispatcher>();
    }
}