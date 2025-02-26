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
using Carcass.Core.Locators;
using Carcass.Http.Providers.UserId.Abstracts;
using Carcass.Logging.Adapters;
using Carcass.Logging.Adapters.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Carcass.Http.Providers.UserId;

/// <summary>
///     Represents a factory responsible for creating instances of <see cref="IHttpUserIdentityProvider" />.
/// </summary>
public sealed class HttpUserIdentityProviderFactory : IHttpUserIdentityProviderFactory
{
    /// <summary>
    ///     An instance of <see cref="LoggerAdapter{TCategoryName}" /> used for logging operations
    ///     within the <see cref="HttpUserIdentityProviderFactory" /> class.
    /// </summary>
    /// <remarks>
    ///     Facilitates structured logging functionality, providing contextual log information
    ///     scoped to the operations of <see cref="HttpUserIdentityProviderFactory" />.
    /// </remarks>
    private readonly LoggerAdapter<HttpUserIdentityProviderFactory> _loggerAdapter;

    /// <summary>
    ///     Factory class responsible for creating instances of HTTP user identity providers.
    /// </summary>
    public HttpUserIdentityProviderFactory(ILoggerAdapterFactory loggerAdapterFactory)
    {
        ArgumentVerifier.NotNull(loggerAdapterFactory, nameof(loggerAdapterFactory));

        _loggerAdapter = loggerAdapterFactory.CreateLoggerAdapter<HttpUserIdentityProviderFactory>();
    }

    /// <summary>
    ///     Attempts to create an instance of an <see cref="IHttpUserIdentityProvider" /> using the available services in the
    ///     application's service provider.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="IHttpUserIdentityProvider" /> if required services are available; otherwise, returns
    ///     <c>null</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if a critical internal error occurs during the service resolution or creation process.
    /// </exception>
    public IHttpUserIdentityProvider? TryCreateHttpUserIdentityProvider()
    {
        IHttpContextAccessor? httpContextAccessor = ServiceProviderLocator
            .Current
            .GetOptionalService<IHttpContextAccessor>();
        if (httpContextAccessor is null)
        {
            _loggerAdapter.LogWarning("No required service of type {0} is registered.",
                nameof(IHttpContextAccessor)
            );

            return null;
        }

        if (httpContextAccessor.HttpContext is null)
        {
            _loggerAdapter.LogWarning("{0} is null.",
                nameof(IHttpContextAccessor.HttpContext)
            );

            return null;
        }

        IHttpUserIdentityProvider? httpUserIdentityProvider = ServiceProviderLocator
            .Current
            .GetOptionalService<IHttpUserIdentityProvider>();
        if (httpUserIdentityProvider is null)
            _loggerAdapter.LogWarning("No required service of type {0} is registered.",
                nameof(IHttpUserIdentityProvider)
            );

        return httpUserIdentityProvider;
    }
}