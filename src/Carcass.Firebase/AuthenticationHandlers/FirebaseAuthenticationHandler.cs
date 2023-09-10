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

using System.Security.Claims;
using System.Text.Encodings.Web;
using Carcass.Core;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Carcass.Firebase.AuthenticationHandlers;

public sealed class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string BearerPrefix = "Bearer ";

    private readonly FirebaseApp _firebaseApp;

    public FirebaseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FirebaseApp firebaseApp
    ) : base(options, logger, encoder, clock)
    {
        ArgumentVerifier.NotNull(firebaseApp, nameof(firebaseApp));

        _firebaseApp = firebaseApp;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        string? authorizationHeaderValue = Context.Request.Headers["Authorization"];
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

            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme));
        }
        catch (Exception exception)
        {
            return AuthenticateResult.Fail(exception);
        }
    }
}