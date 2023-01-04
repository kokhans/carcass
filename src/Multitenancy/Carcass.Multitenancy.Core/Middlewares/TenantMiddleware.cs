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
using Microsoft.AspNetCore.Http;

namespace Carcass.Multitenancy.Core.Middlewares;

public sealed class TenantMiddleware<TTenant> where TTenant : class, ITenant
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        ArgumentVerifier.NotNull(next, nameof(next));

        _next = next;
    }

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