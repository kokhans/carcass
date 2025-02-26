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
using System.Text.Encodings.Web;
using Carcass.Core;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Carcass.Firebase.AspNetCore.AuthenticationHandlers;

/// <summary>
///     Handles the authentication process using Firebase Authentication tokens within an ASP.NET Core application.
/// </summary>
/// <remarks>
///     This authentication handler validates Firebase JWTs provided in the Authorization header of HTTP requests.
///     It integrates with Firebase Admin SDK to verify ID tokens and construct a <see cref="ClaimsPrincipal" /> from
///     the token claims.
/// </remarks>
public sealed class FirebaseAuthenticationHandler : AuthenticationHandler<JwtBearerOptions>
{
    /// <summary>
    ///     A constant string representing the "Bearer" prefix used in Authorization headers for token-based authentication
    ///     schemes.
    /// </summary>
    private const string BearerPrefix = "Bearer ";

    /// <summary>
    ///     Represents an instance of a Firebase application used for interacting with Firebase services.
    ///     This instance is utilized to authenticate and manage Firebase tokens in the application.
    /// </summary>
    /// <remarks>
    ///     This variable is initialized in the constructor of the <see cref="FirebaseAuthenticationHandler" /> class
    ///     and is used to interact with FirebaseAdmin functionalities, such as token verification.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if a <see cref="FirebaseApp" /> instance is not provided during initialization.
    /// </exception>
    private readonly FirebaseApp _firebaseApp;

    /// <summary>
    ///     An authentication handler that integrates Firebase authentication with ASP.NET Core's JWT Bearer authentication
    ///     mechanism.
    /// </summary>
    /// <remarks>
    ///     This handler utilizes FirebaseAdmin SDK for verifying Firebase ID tokens and enables authentication
    ///     using Firebase in an ASP.NET Core application.
    /// </remarks>
    [Obsolete("ISystemClock is obsolete, use TimeProvider on AuthenticationSchemeOptions instead.")]
    public FirebaseAuthenticationHandler(
        IOptionsMonitor<JwtBearerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FirebaseApp firebaseApp
    ) : base(options, logger, encoder, clock)
    {
        ArgumentVerifier.NotNull(firebaseApp, nameof(firebaseApp));

        _firebaseApp = firebaseApp;
    }

    /// <summary>
    ///     Handles the authentication process for Firebase-based authentication.
    ///     Verifies the ID token provided in the Authorization header and constructs
    ///     a ClaimsPrincipal if the token is valid.
    /// </summary>
    /// <returns>
    ///     An <see cref="AuthenticateResult" /> indicating the result of the authentication process.
    ///     Returns <see cref="AuthenticateResult.Success(AuthenticationTicket)" /> if authentication
    ///     is successful, <see cref="AuthenticateResult.NoResult" /> if the Authorization header is not
    ///     present, or <see cref="AuthenticateResult.Fail(Exception)" /> if validation fails.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the Authorization header or token is null.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown if an error occurs during token verification or claims extraction.
    /// </exception>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.TryGetValue(
                "Authorization",
                out StringValues value))
            return AuthenticateResult.NoResult();

        string? authorizationHeaderValue = value;
        if (authorizationHeaderValue is null || !authorizationHeaderValue.StartsWith(BearerPrefix))
            return AuthenticateResult.Fail("Invalid scheme.");

        string idToken = authorizationHeaderValue[BearerPrefix.Length..];

        try
        {
            FirebaseToken firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(idToken);
            ClaimsPrincipal claimsPrincipal = new(new List<ClaimsIdentity>
                {
                    new(firebaseToken.Claims
                            .Select(kvp => new Claim(
                                kvp.Key,
                                kvp.Value.ToString() ?? string.Empty)
                            ).ToList(),
                        nameof(ClaimsIdentity)
                    )
                }
            );

            return AuthenticateResult.Success(
                new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme)
            );
        }
        catch (Exception exception)
        {
            return AuthenticateResult.Fail(exception);
        }
    }
}