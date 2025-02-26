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
using Carcass.Azure.Functions.Accessors.Abstracts;
using Carcass.Core;
using Carcass.Core.Accessors.TenantId.Abstracts;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Firebase.Core.Accessors.Abstracts;
using Carcass.Firebase.Core.Helpers;
using Carcass.Firebase.Core.Models;
using Carcass.Json.Core.Providers.Abstracts;
using Microsoft.Azure.Functions.Worker;

namespace Carcass.Firebase.AzureFunctions.Accessors;

/// <summary>
///     Provides Firebase-based user accessor functionality for Azure Functions.
///     Supports user ID, tenant ID, and Firebase user retrieval.
/// </summary>
public sealed class FirebaseAzureFunctionsUserAccessor : IFirebaseUserAccessor, IUserIdAccessor,
    ITenantIdAccessor
{
    /// <summary>
    ///     Provides access to the current <see cref="Microsoft.Azure.Functions.Worker.FunctionContext" /> within Azure
    ///     Functions.
    ///     Used to obtain contextual information for executions, such as metadata or request-based user details.
    /// </summary>
    private readonly IFunctionContextAccessor _functionContextAccessor;

    /// <summary>
    ///     Provides JSON serialization and deserialization methods used to handle Firebase user data and claims.
    /// </summary>
    /// <remarks>
    ///     This member is utilized internally to facilitate JSON-based operations such as parsing claims
    ///     into FirebaseUser instances within Azure Functions context.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    ///     Thrown during runtime if the JSON provider encounters issues in deserialization or serialization operations.
    /// </exception>
    private readonly IJsonProvider _jsonProvider;

    /// <summary>
    ///     Provides access to Firebase user data, along with user ID and tenant ID, specifically in the context of Azure
    ///     Functions.
    ///     Implements <see cref="IFirebaseUserAccessor" />, <see cref="IUserIdAccessor" />, and
    ///     <see cref="ITenantIdAccessor" />.
    /// </summary>
    public FirebaseAzureFunctionsUserAccessor(
        IFunctionContextAccessor functionContextAccessor,
        IJsonProvider jsonProvider)
    {
        ArgumentVerifier.NotNull(functionContextAccessor, nameof(functionContextAccessor));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));

        _functionContextAccessor = functionContextAccessor;
        _jsonProvider = jsonProvider;
    }

    /// <summary>
    ///     Retrieves information about the Firebase user associated with the current context.
    /// </summary>
    /// <returns>
    ///     A <see cref="FirebaseUser" /> object representing the authenticated Firebase user, or <c>null</c> if no user is
    ///     authenticated.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the <see cref="FunctionContext" /> is unavailable, indicating improper middleware configuration.
    /// </exception>
    public FirebaseUser? GetFirebaseUser()
    {
        FunctionContext? functionContext = _functionContextAccessor.FunctionContext;
        if (functionContext is null)
            throw new InvalidOperationException(
                "FunctionContext is not available. Ensure middleware is configured correctly.");

        if (!functionContext.Items.TryGetValue("UserClaimsPrincipal", out object? userPrincipal) ||
            userPrincipal is not ClaimsPrincipal {Identity.IsAuthenticated: true} claimsPrincipal)
            return null;

        List<Claim> claims = claimsPrincipal.Claims.ToList();

        return FirebaseUserHelper.GetFirebaseUser(claims, _jsonProvider);
    }

    /// <summary>
    ///     Attempts to retrieve the tenant ID associated with the current Firebase user.
    /// </summary>
    /// <returns>
    ///     The tenant ID as a string if available; otherwise, null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if there is an error accessing the Firebase user or the tenant ID cannot be retrieved.
    /// </exception>
    public string? TryGetTenantId() => GetFirebaseUser()?.Tenant;

    /// <summary>
    ///     Attempts to retrieve the user ID of the authenticated Firebase user.
    /// </summary>
    /// <returns>
    ///     The user ID as a string if the Firebase user is authenticated; otherwise, null if no user
    ///     is authenticated or the user ID cannot be retrieved.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the method is called in an unexpected context or without proper initialization
    ///     of the necessary dependencies.
    /// </exception>
    public string? TryGetUserId() => GetFirebaseUser()?.Id;
}