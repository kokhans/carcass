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

using System.Security.Claims;
using Carcass.Core;
using Carcass.Core.Accessors.TenantId.Abstracts;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Firebase.Core.Accessors.Abstracts;
using Carcass.Firebase.Core.Helpers;
using Carcass.Firebase.Core.Models;
using Carcass.Json.Core.Providers.Abstracts;
using Microsoft.AspNetCore.Http;

namespace Carcass.Firebase.AspNetCore.Accessors;

/// <summary>
///     Implements the <see cref="IFirebaseUserAccessor" />, <see cref="IUserIdAccessor" />,
///     and <see cref="ITenantIdAccessor" /> interfaces to provide access to user and tenant data
///     from the current HTTP context, utilizing Firebase claims.
/// </summary>
public sealed class FirebaseAspNetCoreUserAccessor : IFirebaseUserAccessor, IUserIdAccessor, ITenantIdAccessor
{
    /// <summary>
    ///     Provides access to the current HTTP context, enabling retrieval of details
    ///     about the current HTTP request, user information, and related data.
    ///     Typically used for obtaining user claims, request headers, and other
    ///     contextual information in a web application.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <see cref="IHttpContextAccessor" /> instance is not provided
    ///     or initialized correctly.
    /// </exception>
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    ///     Provides an instance of <see cref="IJsonProvider" /> for handling JSON serialization and deserialization.
    /// </summary>
    /// <remarks>
    ///     This is used to facilitate conversions between JSON data and C# objects within the context of Firebase user
    ///     management and other related operations.
    /// </remarks>
    /// <seealso cref="IJsonProvider" />
    private readonly IJsonProvider _jsonProvider;

    /// <summary>
    ///     Provides access to Firebase user information, tenant ID, and user ID for an ASP.NET Core application.
    ///     Implements <see cref="IFirebaseUserAccessor" />, <see cref="IUserIdAccessor" />, and
    ///     <see cref="ITenantIdAccessor" />.
    /// </summary>
    public FirebaseAspNetCoreUserAccessor(IHttpContextAccessor httpContextAccessor, IJsonProvider jsonProvider)
    {
        ArgumentVerifier.NotNull(httpContextAccessor, nameof(httpContextAccessor));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));

        _httpContextAccessor = httpContextAccessor;
        _jsonProvider = jsonProvider;
    }

    /// <summary>
    ///     Retrieves the Firebase user details of the currently authenticated user.
    /// </summary>
    /// <returns>
    ///     A <see cref="FirebaseUser" /> representing the authenticated Firebase user, or null if no user is authenticated.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the operation to retrieve Firebase user details fails due to an unexpected issue.
    /// </exception>
    public FirebaseUser? GetFirebaseUser()
    {
        ClaimsPrincipal? user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity is not {IsAuthenticated: true})
            return null;

        List<Claim> claims = user.Claims.ToList();

        return FirebaseUserHelper.GetFirebaseUser(claims, _jsonProvider);
    }

    /// <summary>
    ///     Attempts to retrieve the tenant identifier associated with the current user.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> representing the tenant ID, or <c>null</c> if no tenant ID is available.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the method is unable to retrieve the Firebase user context.
    /// </exception>
    public string? TryGetTenantId() => GetFirebaseUser()?.Tenant;

    /// <summary>
    ///     Attempts to retrieve the user ID from the Firebase user context.
    /// </summary>
    /// <returns>
    ///     The user ID as a string if available; otherwise, null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when attempting to access the user ID if the underlying Firebase user context is null.
    /// </exception>
    public string? TryGetUserId() => GetFirebaseUser()?.Id;
}