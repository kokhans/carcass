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
using System.Text.Json.Nodes;
using Carcass.Core;
using Carcass.Core.Accessors.TenantId.Abstracts;
using Carcass.Core.Accessors.UserId.Abstracts;
using Carcass.Firebase.Accessors.Abstracts;
using Carcass.Firebase.Models;
using Carcass.Json.Core.Providers.Abstracts;
using Carcass.Mvc.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Carcass.Firebase.Accessors;

public sealed class FirebaseUserAccessor : IFirebaseUserAccessor, IUserIdAccessor, ITenantIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJsonProvider _jsonProvider;

    public FirebaseUserAccessor(IHttpContextAccessor httpContextAccessor, IJsonProvider jsonProvider)
    {
        ArgumentVerifier.NotNull(httpContextAccessor, nameof(httpContextAccessor));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));

        _httpContextAccessor = httpContextAccessor;
        _jsonProvider = jsonProvider;
    }

    public FirebaseUser? GetFirebaseUser()
    {
        ClaimsPrincipal? claimsPrincipal = _httpContextAccessor.HttpContext?.User;

        if (claimsPrincipal?.Identity is { IsAuthenticated: true })
        {
            List<Claim> claims = claimsPrincipal.Claims.ToList();

            string? id = claims.TryGetClaim("user_id");
            string? email = claims.TryGetClaim("email");
            bool emailVerified = bool.Parse(claims.TryGetClaim("email_verified") ?? "false");
            string? username = claims.TryGetClaim("username");

            JsonObject firebase = _jsonProvider.Deserialize<JsonObject>(claims.TryGetClaim("firebase")!);
            string? tenant = null;
            if (firebase.TryGetPropertyValue("tenant", out JsonNode? jsonNode))
                tenant = jsonNode!.GetValue<string>();

            return new FirebaseUser(id, email, emailVerified, username, tenant);
        }

        return null;
    }

    public string? TryGetUserId() => GetFirebaseUser()?.Id;

    public string? TryGetTenantId() => GetFirebaseUser()?.Tenant;
}