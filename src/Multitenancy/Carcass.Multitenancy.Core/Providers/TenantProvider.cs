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
using Carcass.Multitenancy.Core.Entities.Abstracts;
using Carcass.Multitenancy.Core.Providers.Abstracts;
using Carcass.Multitenancy.Core.ResolutionStrategies.Abstracts;
using Carcass.Multitenancy.Core.Stores.Abstracts;

namespace Carcass.Multitenancy.Core.Providers;

/// <summary>
///     Provides functionality for resolving and retrieving tenant information.
/// </summary>
/// <typeparam name="TTenant">The type of the tenant entity, which must implement the <see cref="ITenant" /> interface.</typeparam>
public sealed class TenantProvider<TTenant> : ITenantProvider<TTenant>
    where TTenant : class, ITenant
{
    /// <summary>
    ///     A strategy used to determine the current tenant's identifier in a multitenant application.
    /// </summary>
    private readonly ITenantResolutionStrategy _tenantResolutionStrategy;

    /// <summary>
    ///     Represents the data store responsible for managing tenant information.
    /// </summary>
    /// <typeparam name="TTenant">
    ///     The type that represents a tenant entity, constrained to implement the <see cref="ITenant" /> interface.
    /// </typeparam>
    /// <remarks>
    ///     The tenant store provides functionality to load tenant information from a persistent storage,
    ///     ensuring tenant-specific data can be retrieved when needed in a multitenant application.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the tenant store is not properly initialized.
    /// </exception>
    private readonly ITenantStore<TTenant> _tenantStore;

    /// <summary>
    ///     Provides functionality to resolve and manage tenants.
    /// </summary>
    /// <typeparam name="TTenant">
    ///     The type of the tenant, which must implement the <see cref="ITenant" /> interface.
    /// </typeparam>
    public TenantProvider(ITenantResolutionStrategy tenantResolutionStrategy, ITenantStore<TTenant> tenantStore)
    {
        ArgumentVerifier.NotNull(tenantResolutionStrategy, nameof(tenantResolutionStrategy));
        ArgumentVerifier.NotNull(tenantStore, nameof(tenantStore));

        _tenantResolutionStrategy = tenantResolutionStrategy;
        _tenantStore = tenantStore;
    }

    /// <summary>
    ///     Asynchronously retrieves the current tenant instance based on the tenant resolution strategy and store.
    /// </summary>
    /// <typeparam name="TTenant">The tenant type that implements the <see cref="ITenant" /> interface.</typeparam>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     The tenant instance of type <typeparamref name="TTenant" /> if successfully resolved and loaded; otherwise,
    ///     <c>null</c>.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    /// <exception cref="Exception">
    ///     May throw exceptions related to tenant resolution or storage access failures.
    /// </exception>
    public async Task<TTenant?> GetTenantAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? tenantId = await _tenantResolutionStrategy.GetTenantIdAsync(cancellationToken);

        return tenantId is not null
            ? await _tenantStore.LoadTenantAsync(tenantId, cancellationToken)
            : null;
    }
}