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
using Carcass.Http.Accessors.UserId.Abstracts;
using Carcass.Http.Providers.UserId.Abstracts;

namespace Carcass.Http.Accessors.UserId;

/// <summary>
///     Provides functionality to access the user ID associated with an HTTP request.
///     This class implements <see cref="IHttpUserIdAccessor" /> to allow retrieval
///     of user identification in the context of HTTP scenarios.
/// </summary>
public sealed class HttpUserIdAccessor : IHttpUserIdAccessor
{
    /// <summary>
    ///     Represents an instance of <see cref="IHttpUserIdentityProvider" /> used to retrieve user identity information in
    ///     the context of an HTTP request.
    /// </summary>
    /// <remarks>
    ///     This variable holds the provided implementation of <see cref="IHttpUserIdentityProvider" />, which is created using
    ///     an <see cref="IHttpUserIdentityProviderFactory" />.
    ///     It enables the retrieval of user-related data, such as user ID, in scenarios where HTTP context is applicable.
    /// </remarks>
    private readonly IHttpUserIdentityProvider? _httpUserIdentityProvider;

    /// <summary>
    ///     Provides an implementation of the <see cref="IHttpUserIdAccessor" /> interface for
    ///     accessing the user ID from HTTP user identity providers.
    /// </summary>
    public HttpUserIdAccessor(IHttpUserIdentityProviderFactory httpUserIdentityProviderFactory)
    {
        ArgumentVerifier.NotNull(httpUserIdentityProviderFactory, nameof(httpUserIdentityProviderFactory));

        _httpUserIdentityProvider = httpUserIdentityProviderFactory.TryCreateHttpUserIdentityProvider();
    }

    /// <summary>
    ///     Attempts to retrieve the user ID from the HTTP user identity provider, if available.
    /// </summary>
    /// <returns>
    ///     The user ID as a string if it can be retrieved; otherwise, null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the HTTP user identity provider cannot be created or accessed.
    /// </exception>
    public string? TryGetUserId() => _httpUserIdentityProvider?.TryGetUserId();
}