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

namespace Carcass.Multitenancy.Core.Providers.Abstracts;

/// <summary>
///     Provides an abstraction for retrieving tenant information in a multitenant application.
/// </summary>
/// <typeparam name="TTenant">
///     The type that represents a tenant and must implement the <see cref="ITenant" /> interface.
/// </typeparam>
public interface ITenantProvider<TTenant> where TTenant : class, ITenant
{
    // ReSharper disable once UnusedMemberInSuper.Global
    /// <summary>
    ///     Retrieves a tenant asynchronously based on the resolved tenant identifier.
    /// </summary>
    /// <typeparam name="TTenant">The type of the tenant that implements the <see cref="ITenant" /> interface.</typeparam>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///     The tenant corresponding to the resolved identifier, or <c>null</c> if no tenant is found.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided <paramref name="cancellationToken" />.
    /// </exception>
    /// <exception cref="Exception">
    ///     May throw exceptions based on the implementation of the tenant resolution or store.
    /// </exception>
    Task<TTenant?> GetTenantAsync(CancellationToken cancellationToken = default);
}