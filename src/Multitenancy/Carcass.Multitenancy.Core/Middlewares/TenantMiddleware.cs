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
using Carcass.Multitenancy.Core.Providers;
using Microsoft.AspNetCore.Http;

namespace Carcass.Multitenancy.Core.Middlewares;

/// <summary>
///     Middleware that handles multitenancy by resolving the current tenant
///     and injecting it into the HTTP context.
/// </summary>
/// <typeparam name="TTenant">
///     The type representing the tenant, which must implement <see cref="ITenant" />.
/// </typeparam>
public sealed class TenantMiddleware<TTenant> where TTenant : class, ITenant
{
    /// <summary>
    ///     Represents the next middleware in the HTTP request pipeline.
    ///     This delegate is invoked to proceed with the subsequent middleware or the terminal processing of the request.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Middleware responsible for tenant resolution in a multi-tenant application.
    ///     Processes incoming HTTP requests and ensures tenant-related logic is applied.
    /// </summary>
    /// <typeparam name="TTenant">The type representing a tenant, constrained to implement the ITenant interface.</typeparam>
    public TenantMiddleware(RequestDelegate next)
    {
        ArgumentVerifier.NotNull(next, nameof(next));

        _next = next;
    }

    /// <summary>
    ///     Middleware to manage tenants in the application by resolving and setting the tenant information
    ///     into the current HTTP context.
    /// </summary>
    /// <typeparam name="TTenant">The type of the tenant entity which implements <see cref="ITenant" />.</typeparam>
    /// <param name="httpContext">The current HTTP context associated with the middleware pipeline.</param>
    /// <returns>A <see cref="Task" /> representing the completion of the middleware's processing logic.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="httpContext" /> is null.
    /// </exception>
    public async Task Invoke(HttpContext httpContext)
    {
        ArgumentVerifier.NotNull(httpContext, nameof(httpContext));

        if (!httpContext.Items.ContainsKey("Tenant"))
        {
            if (httpContext.RequestServices.GetService(typeof(TenantProvider<TTenant>))
                is TenantProvider<TTenant> tenantAccessService
               ) httpContext.Items.Add("Tenant", await tenantAccessService.GetTenantAsync());
        }

        await _next(httpContext);
    }
}