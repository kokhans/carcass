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

namespace Carcass.Http.Providers.UserId.Abstracts;

/// <summary>
///     Defines a factory interface responsible for creating instances of <see cref="IHttpUserIdentityProvider" />.
/// </summary>
public interface IHttpUserIdentityProviderFactory
{
    /// <summary>
    ///     Attempts to create an instance of an <see cref="IHttpUserIdentityProvider" /> for retrieving
    ///     the User ID associated with the current HTTP request context.
    /// </summary>
    /// <returns>
    ///     An instance of <see cref="IHttpUserIdentityProvider" /> if the appropriate services
    ///     are registered; otherwise, null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if a required service for creating the <see cref="IHttpUserIdentityProvider" /> is inaccessible
    ///     or improperly configured.
    /// </exception>
    IHttpUserIdentityProvider? TryCreateHttpUserIdentityProvider();
}