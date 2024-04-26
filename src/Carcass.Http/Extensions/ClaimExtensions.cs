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

namespace Carcass.Http.Extensions;

/// <summary>
///     Provides extension methods for working with claims in the <see cref="System.Security.Claims.Claim" /> class.
/// </summary>
public static class ClaimExtensions
{
    /// <summary>
    ///     Retrieves a claim value with the specified claim type from the provided list of claims.
    /// </summary>
    /// <param name="source">The list of claims to search within. Can be null.</param>
    /// <param name="claimType">The type of claim to search for. Cannot be null or empty.</param>
    /// <returns>
    ///     The value of the claim if found, or null if the list is null, empty,
    ///     or if the specified claim type is not found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="claimType" /> is null or whitespace.
    /// </exception>
    public static string? TryGetClaim(this IList<Claim>? source, string claimType)
    {
        ArgumentVerifier.NotNull(claimType, nameof(claimType));

        if (source is null || source.Count == 0)
            return null;

        string? claim = source.SingleOrDefault(c =>
            c.Type.Equals(claimType, StringComparison.InvariantCultureIgnoreCase)
        )?.Value;

        return !string.IsNullOrWhiteSpace(claim) ? claim : null;
    }
}