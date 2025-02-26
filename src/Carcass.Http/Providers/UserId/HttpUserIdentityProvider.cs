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
using Carcass.Http.Extensions;
using Carcass.Http.Providers.UserId.Abstracts;
using Carcass.Http.Settings;
using Carcass.Logging.Adapters;
using Carcass.Logging.Adapters.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Carcass.Http.Providers.UserId;

/// <summary>
///     Provides functionality to retrieve the User ID of the currently authenticated user from the HTTP context.
///     Implements the <see cref="IHttpUserIdentityProvider" /> interface.
/// </summary>
public sealed class HttpUserIdentityProvider : IHttpUserIdentityProvider
{
    /// <summary>
    ///     Provides access to the current HTTP context for the purpose of retrieving information
    ///     about the incoming HTTP request and user details, including user claims.
    /// </summary>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Responsible for handling logging operations within the context of the
    ///     HttpUserIdentityProvider, providing a structured way to log messages
    ///     and events.
    /// </summary>
    /// <remarks>
    ///     Utilizes the <see cref="LoggerAdapter{TCategoryName}" /> to facilitate integration
    ///     with the Microsoft.Extensions.Logging framework for logging activities specific
    ///     to the <see cref="HttpUserIdentityProvider" />.
    /// </remarks>
    private readonly LoggerAdapter<HttpUserIdentityProvider> _loggerAdapter;

    /// <summary>
    ///     Represents the settings used to configure the <see cref="HttpUserIdentityProvider" />.
    /// </summary>
    private readonly HttpUserIdentityProviderSettings _settings;

    /// <summary>
    ///     Provides functionality to retrieve the current user's identity from an HTTP context based on specific settings.
    /// </summary>
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

    /// <summary>
    ///     Attempts to retrieve the user ID based on the specified claim type settings.
    /// </summary>
    /// <returns>
    ///     The user ID if successfully resolved; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the user ID claim type setting is null or not properly configured.
    /// </exception>
    public string? TryGetUserId() => TryGet(_settings.UserIdClaim);

    /// <summary>
    ///     Retrieves the value of a specified claim type from the current HTTP user context.
    /// </summary>
    /// <param name="claimType">The type of claim to retrieve from the user's claims.</param>
    /// <returns>
    ///     The value of the claim if found; otherwise, null if the claim does not exist or is invalid.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the claimType parameter is null or empty.</exception>
    private string? TryGet(string claimType)
    {
        ArgumentVerifier.NotNull(claimType, nameof(claimType));

        string? claim = _httpContextAccessor.HttpContext?.User.Claims.ToList().TryGetClaim(claimType);
        if (string.IsNullOrWhiteSpace(claim))
            _loggerAdapter.LogWarning("Claim {0} not found.", claimType);

        return claim;
    }
}