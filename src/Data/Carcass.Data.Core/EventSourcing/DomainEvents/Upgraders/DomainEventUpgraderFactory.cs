// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders;

public sealed class DomainEventUpgraderFactory : IDomainEventUpgraderFactory
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DomainEventUpgraderFactory(IServiceScopeFactory serviceScopeFactory)
    {
        ArgumentVerifier.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

        _serviceScopeFactory = serviceScopeFactory;
    }

    public IDomainEventUpgrader? GetDomainEventUpgrader(Type domainEventType)
    {
        ArgumentVerifier.NotNull(domainEventType, nameof(domainEventType));

        if (!string.IsNullOrWhiteSpace(domainEventType.FullName))
        {
            using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
            IServiceProvider serviceProvider = serviceScope.ServiceProvider;
            DependencyStore<Type> dependencyStore = serviceProvider.GetRequiredService<DependencyStore<Type>>();
            Type? eventUpgraderType = dependencyStore.GetOptionalDependency(domainEventType.FullName);
            if (eventUpgraderType is not null)
                if (serviceProvider.GetRequiredService(eventUpgraderType) is IDomainEventUpgrader domainEventUpgrader)
                    return domainEventUpgrader;
        }

        return null;
    }
}