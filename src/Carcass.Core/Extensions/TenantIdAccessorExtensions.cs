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

using Carcass.Core.Accessors.TenantId.Abstracts;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Carcass.Core.Extensions;

/// <summary>
///     Provides extension methods for working with the <see cref="ITenantIdAccessor" /> interface to retrieve a tenant ID.
/// </summary>
public static class TenantIdAccessorExtensions
{
    /// <summary>
    ///     Retrieves the tenant ID from the provided <see cref="ITenantIdAccessor" />.
    /// </summary>
    /// <param name="tenantIdAccessor">The accessor used to retrieve the tenant ID.</param>
    /// <returns>The tenant ID as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="tenantIdAccessor" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the tenant ID is null, empty, or consists only of whitespace.</exception>
    public static string GetTenantId(this ITenantIdAccessor tenantIdAccessor)
    {
        ArgumentVerifier.NotNull(tenantIdAccessor, nameof(tenantIdAccessor));

        string? tenantId = tenantIdAccessor.TryGetTenantId();

        if (string.IsNullOrWhiteSpace(tenantId))
            throw new InvalidOperationException("TenantId is null.");

        return tenantId;
    }
}