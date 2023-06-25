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
using Carcass.Multitenancy.Core.Entities.Abstracts;
using Carcass.Multitenancy.Core.Providers;
using Carcass.Multitenancy.Core.ResolutionStrategies.Abstracts;
using Carcass.Multitenancy.Core.Stores.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Carcass.Multitenancy.Core.Builders;

public sealed class TenantBuilder<TTenant> where TTenant : class, ITenant
{
    private readonly IServiceCollection _services;

    public TenantBuilder(IServiceCollection services)
    {
        ArgumentVerifier.NotNull(services, nameof(services));

        services.AddTransient<TenantProvider<TTenant>>();
        _services = services;
    }

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