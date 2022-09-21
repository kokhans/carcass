// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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
using Carcass.Data.Core.Aggregates.ResolutionStrategies;
using Carcass.Data.Core.Aggregates.ResolutionStrategies.Abstracts;
using Carcass.Data.Core.Commands.Dispatchers;
using Carcass.Data.Core.Commands.Dispatchers.Abstracts;
using Carcass.Data.Core.Commands.Handlers.Abstracts;
using Carcass.Data.Core.Commands.Notifications.Dispatchers;
using Carcass.Data.Core.Commands.Notifications.Dispatchers.Abstracts;
using Carcass.Data.Core.Commands.Notifications.Stores;
using Carcass.Data.Core.Commands.Notifications.Stores.Abstracts;
using Carcass.Data.Core.Commands.Validators.Abstracts;
using Carcass.Data.Core.DomainEvents.Locators;
using Carcass.Data.Core.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.DomainEvents.Upgraders;
using Carcass.Data.Core.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.Core.Queries.Dispatchers;
using Carcass.Data.Core.Queries.Dispatchers.Abstracts;
using Carcass.Data.Core.Queries.Handlers.Abstracts;
using Scrutor;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCarcassInMemoryCommandDispatcher(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>();
    }

    public static IServiceCollection AddCarcassInMemoryQueryDispatcher(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
    }

    public static IServiceCollection AddCarcassInMemoryNotificationStore(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<INotificationStore, InMemoryNotificationStore>();
    }

    public static IServiceCollection AddCarcassInMemoryNotificationDispatcher(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<INotificationDispatcher, InMemoryNotificationDispatcher>();
    }

    public static IServiceCollection AddCarcassCapitalizedAggregateNameResolutionStrategy(
        this IServiceCollection services
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services.AddSingleton<IAggregateNameResolutionStrategy, CapitalizedAggregateNameResolutionStrategy>();
    }

    public static IServiceCollection AddCarcassDomainEvents(this IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        return services
            .AddSingleton<IDomainEventLocator, DomainEventLocator>()
            .AddSingleton<IDomainEventUpgraderFactory, DomainEventUpgraderFactory>()
            .AddSingleton<IDomainEventUpgraderDispatcher, DomainEventUpgraderDispatcher>();
    }

    public static IServiceCollection ScanCarcassCommandsAndQueries(
        this IServiceCollection services,
        Func<ITypeSourceSelector, IImplementationTypeSelector> scanFunc
    )
    {
        ArgumentVerifier.NotNull(services, nameof(services));
        ArgumentVerifier.NotNull(scanFunc, nameof(scanFunc));

        return services.Scan(tss => scanFunc(tss)
            .AddClasses(itp => itp.AssignableTo(typeof(ICommandValidator<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsSelfWithInterfaces()
            .WithSingletonLifetime()
            .AddClasses(itp => itp.AssignableTo(typeof(ICommandHandler<,,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
            .AddClasses(itp => itp.AssignableTo(typeof(IQueryHandler<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithSingletonLifetime()
        );
    }
}