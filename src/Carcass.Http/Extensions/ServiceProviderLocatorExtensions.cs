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

namespace Carcass.Http.Extensions;

// ReSharper disable once UnusedType.Global
/// <summary>
///     Provides extension methods for <see cref="ServiceProviderLocator" /> to interact with HTTP user identity providers.
/// </summary>
public static class ServiceProviderLocatorExtensions
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Attempts to retrieve an instance of <see cref="IHttpUserIdentityProvider" /> from the provided service locator.
    /// </summary>
    /// <param name="serviceProviderLocator">
    ///     The <see cref="ServiceProviderLocator" /> instance used to locate the service.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="IHttpUserIdentityProvider" /> if found; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="serviceProviderLocator" /> is null.
    /// </exception>
    public static IHttpUserIdentityProvider? TryGetHttpUserIdentityProvider(
        this ServiceProviderLocator serviceProviderLocator
    )
    {
        ArgumentVerifier.NotNull(serviceProviderLocator, nameof(serviceProviderLocator));

        IHttpUserIdentityProviderFactory? httpUserIdentityProviderFactory =
            serviceProviderLocator.GetOptionalService<IHttpUserIdentityProviderFactory>();

        return httpUserIdentityProviderFactory?.TryCreateHttpUserIdentityProvider();
    }
}