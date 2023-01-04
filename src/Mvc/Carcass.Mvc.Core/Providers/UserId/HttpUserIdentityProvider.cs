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
using Carcass.Logging.Core.Adapters;
using Carcass.Logging.Core.Adapters.Abstracts;
using Carcass.Mvc.Core.Extensions;
using Carcass.Mvc.Core.Providers.UserId.Abstracts;
using Carcass.Mvc.Core.Settings;
using Microsoft.AspNetCore.Http;

namespace Carcass.Mvc.Core.Providers.UserId;

public sealed class HttpUserIdentityProvider : IHttpUserIdentityProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpUserIdentityProviderSettings _settings;
    private readonly LoggerAdapter<HttpUserIdentityProvider> _loggerAdapter;

    public HttpUserIdentityProvider(
        ILoggerAdapterFactory loggerAdapterFactory,
        IHttpContextAccessor httpContextAccessor,
        HttpUserIdentityProviderSettings settings
    )
    {
        ArgumentVerifier.NotNull(loggerAdapterFactory, nameof(loggerAdapterFactory));
        ArgumentVerifier.NotNull(httpContextAccessor, nameof(httpContextAccessor));
        ArgumentVerifier.NotNull(settings, nameof(settings));

        _httpContextAccessor = httpContextAccessor;
        _settings = settings;
        _loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<HttpUserIdentityProvider>();
    }

    public string? TryGetUserId() => TryGet(_settings.UserIdClaim);

    private string? TryGet(string claimType)
    {
        ArgumentVerifier.NotNull(claimType, nameof(claimType));

        string? claim = _httpContextAccessor.HttpContext?.User.Claims.ToList().TryGetClaim(claimType);
        if (string.IsNullOrWhiteSpace(claim))
            _loggerAdapter.LogWarning("Claim {0} not found.", claimType);

        return claim;
    }
}