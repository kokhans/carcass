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
using Carcass.Multitenancy.Core.ResolutionStrategies.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Carcass.Multitenancy.Core.ResolutionStrategies;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Resolves a tenant identifier from the host of the current HTTP request.
/// </summary>
public sealed class HostTenantResolutionStrategy : ITenantResolutionStrategy
{
    /// <summary>
    ///     Provides access to the current <see cref="HttpContext" /> for retrieving information related to the ongoing HTTP
    ///     request.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Implements a tenant resolution strategy based on the host of the incoming request.
    /// </summary>
    public HostTenantResolutionStrategy(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentVerifier.NotNull(httpContextAccessor, nameof(httpContextAccessor));

        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    ///     Retrieves the tenant identifier for the current request context asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains the tenant identifier as a string,
    ///     or null if the tenant identifier cannot be resolved.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled using the provided <paramref name="cancellationToken" />.
    /// </exception>
    public Task<string?> GetTenantIdAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(_httpContextAccessor.HttpContext?.Request.Host.Host);
    }
}