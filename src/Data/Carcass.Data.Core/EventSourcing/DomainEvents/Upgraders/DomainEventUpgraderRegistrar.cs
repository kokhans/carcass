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
using Carcass.Core.Dependencies;
using Carcass.Data.Core.EventSourcing.DomainEvents.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMember.Global

namespace Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Facilitates the registration and management of domain event upgraders, ensuring that
///     the appropriate upgrader implementations can be resolved and utilized within an
///     event-sourced system.
/// </summary>
public sealed class DomainEventUpgraderRegistrar
{
    /// <summary>
    ///     Stores and manages dependencies of domain event upgraders,
    ///     keyed by the full name of the associated domain event type.
    /// </summary>
    /// <remarks>
    ///     This instance is responsible for maintaining mappings between domain event types and their associated
    ///     upgraders to facilitate the registration process in dependency injection.
    /// </remarks>
    private readonly DependencyStore<Type> _dependencyStore = new();

    /// <summary>
    ///     Registers an upgrader for a specific domain event type to facilitate its transformation
    ///     during event sourcing updates.
    /// </summary>
    /// <typeparam name="TDomainEvent">The type of the domain event being upgraded.</typeparam>
    /// <typeparam name="TDomainEventUpgrader">
    ///     The type of the upgrader that implements <see cref="IDomainEventUpgrader" /> for the specified
    ///     domain event type.
    /// </typeparam>
    /// <returns>
    ///     Returns the current instance of <see cref="DomainEventUpgraderRegistrar" /> for method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the fully qualified name of <typeparamref name="TDomainEvent" /> is null or whitespace.
    /// </exception>
    public DomainEventUpgraderRegistrar AddDomainEventUpgrader<TDomainEvent, TDomainEventUpgrader>()
        where TDomainEvent : IDomainEvent
        where TDomainEventUpgrader : class, IDomainEventUpgrader
    {
        string? domainEventFullName = typeof(TDomainEvent).FullName;
        if (!string.IsNullOrWhiteSpace(domainEventFullName))
            _dependencyStore.AddDependency(domainEventFullName, typeof(TDomainEventUpgrader));

        return this;
    }

    /// <summary>
    ///     Registers domain event upgraders into the provided service collection.
    /// </summary>
    /// <param name="services">The service collection to register the upgraders into.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="services" /> parameter is null.</exception>
    public void Register(IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        foreach (KeyValuePair<string, Type> dependency in _dependencyStore.GetDependencies())
            services.AddSingleton(dependency.Value);

        services.AddSingleton(_dependencyStore);
    }
}