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

using System.Collections.Concurrent;
using Carcass.Core;
using Carcass.Multitenancy.Core.Entities.Abstracts;
using Carcass.Multitenancy.Core.Stores.Abstracts;

namespace Carcass.Multitenancy.Core.Stores;

/// <summary>
///     Represents an in-memory implementation of the <see cref="ITenantStore{TTenant}" /> interface for managing tenant
///     information.
/// </summary>
/// <typeparam name="TTenant">
///     The type of tenant being managed, which must implement the <see cref="ITenant" /> interface.
/// </typeparam>
public sealed class InMemoryTenantStore<TTenant> : ITenantStore<TTenant>
    where TTenant : class, ITenant
{
    /// <summary>
    ///     Represents an in-memory collection of tenant entities used by the <see cref="InMemoryTenantStore{TTenant}" />.
    /// </summary>
    /// <remarks>
    ///     This collection is intended to store tenant instances in a concurrent and thread-safe manner using a
    ///     <see cref="ConcurrentBag{T}" />.
    /// </remarks>
    /// <typeparam name="TTenant">
    ///     The type of tenant being managed, which must implement the <see cref="ITenant" /> interface.
    /// </typeparam>
    private readonly ConcurrentBag<TTenant> _tenants = [];

    /// <summary>
    ///     Asynchronously loads a tenant by its identifier from the in-memory tenant store.
    /// </summary>
    /// <typeparam name="TTenant">The type of the tenant implementing the <see cref="ITenant" /> interface.</typeparam>
    /// <param name="tenantId">The identifier of the tenant to load. Cannot be null.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the operation. The task result may contain the tenant or null if the tenant is not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="tenantId" /> is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public Task<TTenant?> LoadTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(tenantId, nameof(tenantId));

        _tenants.TryPeek(out TTenant? tenant);

        return Task.FromResult(tenant);
    }

    /// <summary>
    ///     Saves a tenant to the in-memory tenant store.
    /// </summary>
    /// <param name="tenant">The tenant instance to save. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous save operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="tenant" /> parameter is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public Task SaveTenantAsync(TTenant tenant, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(tenant, nameof(tenant));

        _tenants.Add(tenant);

        return Task.CompletedTask;
    }
}