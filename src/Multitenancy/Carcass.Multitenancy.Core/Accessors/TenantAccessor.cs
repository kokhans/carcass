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
using Carcass.Multitenancy.Core.Accessors.Abstracts;
using Carcass.Multitenancy.Core.Entities.Abstracts;
using Carcass.Multitenancy.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Carcass.Multitenancy.Core.Accessors;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides mechanisms to access tenant-related information, such as the current tenant
///     or tenant identifier, in multitenant applications.
/// </summary>
/// <typeparam name="TTenant">
///     The type representing the tenant, which must implement the <see cref="ITenant" /> interface.
/// </typeparam>
public sealed class TenantAccessor<TTenant> : ITenantAccessor<TTenant> where TTenant : class, ITenant
{
    /// <summary>
    ///     Provides access to the current HTTP context through an implementation of <see cref="IHttpContextAccessor" />.
    ///     This is utilized to retrieve tenant-related data or other context-specific information
    ///     tied to the HTTP request within the multitenancy framework.
    /// </summary>
    /// <remarks>
    ///     This field holds the instance of <see cref="IHttpContextAccessor" />, which encapsulates
    ///     the current HTTP context and allows safe access within various application components.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided <see cref="IHttpContextAccessor" /> instance is null during
    ///     the construction of containing class.
    /// </exception>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Provides methods to access tenant-related information, such as retrieving the tenant or its identifier,
    ///     from the current HTTP context.
    /// </summary>
    /// <typeparam name="TTenant">The tenant type that implements the <see cref="ITenant" /> interface.</typeparam>
    public TenantAccessor(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentVerifier.NotNull(httpContextAccessor, nameof(httpContextAccessor));

        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Attempts to retrieve the tenant identifier.
    /// </summary>
    /// <returns>
    ///     The tenant identifier if a tenant is available, or null if no tenant is found.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the tenant instance is invalid or the identifier is inaccessible.
    /// </exception>
    public string? TryGetTenantId() => TryGetTenant()?.Identifier;

    /// <summary>
    ///     Attempts to retrieve the current tenant from the HTTP context.
    /// </summary>
    /// <typeparam name="TTenant">
    ///     The type of the tenant, which must implement the <see cref="ITenant" /> interface.
    /// </typeparam>
    /// <returns>
    ///     The current tenant of type <typeparamref name="TTenant" /> if found; otherwise, null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the HTTP context is null.
    /// </exception>
    public TTenant? TryGetTenant() => _httpContextAccessor.HttpContext?.GetTenant<TTenant>();
}