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
using Microsoft.AspNetCore.Http;

namespace Carcass.Multitenancy.Core.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="HttpContext" /> class, specifically for working with multitenancy
///     scenarios.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    ///     Retrieves the tenant of the specified type from the HTTP context, if available.
    /// </summary>
    /// <typeparam name="TTenant">
    ///     The type of the tenant, which must implement the <see cref="ITenant" /> interface.
    /// </typeparam>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <returns>
    ///     The tenant of type <typeparamref name="TTenant" /> if found; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the provided <paramref name="httpContext" /> is null.
    /// </exception>
    public static TTenant? GetTenant<TTenant>(this HttpContext httpContext)
        where TTenant : class, ITenant
    {
        ArgumentVerifier.NotNull(httpContext, nameof(httpContext));

        if (!httpContext.Items.TryGetValue("Tenant", out object? item))
            return null;

        return item as TTenant;
    }
}