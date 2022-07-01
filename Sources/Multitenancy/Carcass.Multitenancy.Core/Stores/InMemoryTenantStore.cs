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

using System.Collections.Concurrent;
using Carcass.Core;
using Carcass.Multitenancy.Core.Entities.Abstracts;
using Carcass.Multitenancy.Core.Stores.Abstracts;

namespace Carcass.Multitenancy.Core.Stores;

public sealed class InMemoryTenantStore<TTenant> : ITenantStore<TTenant>
    where TTenant : class, ITenant
{
    private readonly ConcurrentBag<TTenant> _tenants;

    public InMemoryTenantStore()
    {
        _tenants = new ConcurrentBag<TTenant>();
    }

    public Task<TTenant?> LoadTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(tenantId, nameof(tenantId));

        _tenants.TryPeek(out TTenant? tenant);

        return Task.FromResult(tenant);
    }

    public Task SaveTenantAsync(TTenant tenant, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(tenant, nameof(tenant));

        _tenants.Add(tenant);

        return Task.CompletedTask;
    }
}