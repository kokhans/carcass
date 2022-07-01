// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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
using Carcass.Logging.Core.Adapters;
using Carcass.Logging.Core.Adapters.Abstracts;
using Carcass.Mvc.Core.Extensions;
using Carcass.Mvc.Core.Providers.Abstracts;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Carcass.Mvc.Core.Providers;

public sealed class HttpUserIdentityProvider : IHttpUserIdentityProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerAdapter<HttpUserIdentityProvider> _loggerAdapter;

    public HttpUserIdentityProvider(
        ILoggerAdapterFactory loggerAdapterFactory,
        IHttpContextAccessor httpContextAccessor
    )
    {
        ArgumentVerifier.NotNull(loggerAdapterFactory, nameof(loggerAdapterFactory));
        ArgumentVerifier.NotNull(httpContextAccessor, nameof(httpContextAccessor));

        _loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<HttpUserIdentityProvider>();
        _httpContextAccessor = httpContextAccessor;
    }


    public string? TryGetUserId() => TryGet(JwtClaimTypes.Subject);

    public string? TryGetUserName() => TryGet(JwtClaimTypes.PreferredUserName);

    public string? TryGetUserEmail() => TryGet(JwtClaimTypes.Email);

    public string? TryGetUserPhoneNumber() => TryGet(JwtClaimTypes.PhoneNumber);

    public async Task<string?> GetTokenAsync(
        string schema = AuthenticationSchema.Bearer,
        string tokenName = Token.AccessToken,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(schema, nameof(schema));
        ArgumentVerifier.NotNull(tokenName, nameof(tokenName));

        return _httpContextAccessor.HttpContext is not null
            ? await _httpContextAccessor.HttpContext.GetTokenAsync(schema, tokenName)
            : null;
    }

    private string? TryGet(string claimType)
    {
        ArgumentVerifier.NotNull(claimType, nameof(claimType));

        string? claim = _httpContextAccessor.HttpContext?.User.Claims.ToList().TryGetClaim(claimType);
        if (string.IsNullOrWhiteSpace(claim))
            _loggerAdapter.LogWarning("Claim {0} not found.", claimType);

        return claim;
    }
}