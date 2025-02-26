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
using Carcass.Data.Core.EventSourcing.Aggregates.ResolutionStrategies;
using Carcass.Data.Core.EventSourcing.Aggregates.ResolutionStrategies.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;

// ReSharper disable UnusedMember.Global
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for registering Carcass Event Sourcing services
///     in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Adds the CapitalizedAggregateNameResolutionStrategy implementation of IAggregateNameResolutionStrategy
    ///     to the specified IServiceCollection. This strategy is used for resolving aggregate names in event sourcing.
    /// </summary>
    /// <param name="services">
    ///     The IServiceCollection to which the CapitalizedAggregateNameResolutionStrategy will be added.
    /// </param>
    /// <returns>
    ///     The same IServiceCollection instance with the CapitalizedAggregateNameResolutionStrategy registered.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided services argument is null.
    /// </exception>
    public static IServiceCollection AddCarcassEventSourcingCapitalizedAggregateNameResolutionStrategy(
        this IServiceCollection services
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IAggregateNameResolutionStrategy, CapitalizedAggregateNameResolutionStrategy>();
    }

    /// <summary>
    ///     Adds services related to Carcass event sourcing domain events to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the services will be added.</param>
    /// <returns>The updated service collection with the domain event services registered.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="services" /> is null.</exception>
    public static IServiceCollection AddCarcassEventSourcingDomainEvents(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services
            .AddSingleton<IDomainEventLocator, DomainEventLocator>()
            .AddSingleton<IDomainEventUpgraderFactory, DomainEventUpgraderFactory>()
            .AddSingleton<IDomainEventUpgraderDispatcher, DomainEventUpgraderDispatcher>();
    }
}