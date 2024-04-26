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

using Carcass.Multitenancy.Core.Entities.Abstracts;

namespace Carcass.Multitenancy.Core.Stores.Abstracts;

/// <summary>
///     Represents a store for managing tenant information in a multitenant application.
/// </summary>
/// <typeparam name="TTenant">
///     The type of tenant being managed, which must implement the <see cref="ITenant" /> interface.
/// </typeparam>
public interface ITenantStore<TTenant> where TTenant : class, ITenant
{
    /// <summary>
    ///     Loads a tenant asynchronously based on the provided tenant ID.
    /// </summary>
    /// <param name="tenantId">The unique identifier of the tenant to be loaded.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result contains the tenant object if found, or null if no
    ///     tenant matches the given ID.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="tenantId" /> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task<TTenant?> LoadTenantAsync(string tenantId, CancellationToken cancellationToken = default);

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Saves the specified tenant entity to the tenant store.
    /// </summary>
    /// <param name="tenant">
    ///     The tenant entity to save. Cannot be null.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to notify operations to cancel. Defaults to <see cref="CancellationToken.None" /> if not provided.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the tenant parameter is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the provided <paramref name="cancellationToken" />.
    /// </exception>
    Task SaveTenantAsync(TTenant tenant, CancellationToken cancellationToken = default);
}