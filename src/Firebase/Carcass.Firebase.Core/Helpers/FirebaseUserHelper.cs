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
using System.Text.Json.Nodes;
using Carcass.Core;
using Carcass.Firebase.Core.Models;
using Carcass.Http.Extensions;
using Carcass.Json.Core.Providers.Abstracts;

namespace Carcass.Firebase.Core.Helpers;

/// <summary>
///     A helper class for managing Firebase user-related operations.
/// </summary>
public static class FirebaseUserHelper
{
    /// <summary>
    ///     Constructs a <see cref="FirebaseUser" /> instance from the provided claims collection.
    /// </summary>
    /// <param name="claims">The list of claims representing the user's information. Cannot be null.</param>
    /// <param name="jsonProvider">The JSON provider used to deserialize nested claim properties. Cannot be null.</param>
    /// <returns>A <see cref="FirebaseUser" /> instance containing user's details parsed from the claims.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="claims" /> or <paramref name="jsonProvider" /> is
    ///     null.
    /// </exception>
    /// <exception cref="FormatException">Thrown if the "email_verified" claim is not a valid Boolean value.</exception>
    public static FirebaseUser GetFirebaseUser(List<Claim> claims, IJsonProvider jsonProvider)
    {
        ArgumentVerifier.NotNull(claims, nameof(claims));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));

        string? id = claims.TryGetClaim("user_id");
        string? email = claims.TryGetClaim("email");
        bool emailVerified = bool.Parse(claims.TryGetClaim("email_verified") ?? "false");
        string? username = claims.TryGetClaim("username");

        JsonObject firebase = jsonProvider.Deserialize<JsonObject>(claims.TryGetClaim("firebase")!);
        string? tenant = null;
        if (firebase.TryGetPropertyValue("tenant", out JsonNode? jsonNode))
            tenant = jsonNode!.GetValue<string>();

        return new FirebaseUser(id, email, emailVerified, username, tenant);
    }
}