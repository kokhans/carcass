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

// ReSharper disable UnusedMember.Global

namespace Carcass.Firebase.Core.Models;

/// <summary>
///     Represents a Firebase user, typically authenticated via Firebase.
///     Contains user-related information such as ID, email, username, and tenant.
/// </summary>
public sealed class FirebaseUser(
    string? id,
    string? email,
    bool emailVerified,
    string? username,
    string? tenant
)
{
    /// <summary>
    ///     Gets the unique identifier for the Firebase user.
    /// </summary>
    /// <returns>
    ///     A string representing the unique identifier of the user, or null if not available.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the identifier is accessed in an invalid state or context.
    /// </exception>
    public string? Id { get; } = id;

    /// <summary>
    ///     Gets the email address associated with the user.
    /// </summary>
    /// <value>
    ///     The user's email address as a string. This may be null if an email address is not set.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the email property is accessed in an unexpected state.
    /// </exception>
    public string? Email { get; } = email;

    /// <summary>
    ///     Indicates whether the user's email address has been verified.
    /// </summary>
    /// <returns>
    ///     A boolean value where <c>true</c> indicates that the email address associated with the user
    ///     has been successfully verified, and <c>false</c> otherwise.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the property is accessed before the user has been properly initialized or retrieved.
    /// </exception>
    public bool EmailVerified { get; } = emailVerified;

    /// <summary>
    ///     Gets the username associated with the Firebase user.
    /// </summary>
    /// <value>
    ///     The username of the Firebase user, or <c>null</c> if it is not set.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if an attempt is made to access the property and the user data is not properly initialized.
    /// </exception>
    public string? Username { get; } = username;

    /// <summary>
    ///     Gets the tenant identifier associated with the user.
    /// </summary>
    /// <value>
    ///     A <see cref="string" /> that uniquely identifies the user's tenant, or <c>null</c> if no tenant is associated.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the tenant information cannot be retrieved due to an invalid or unavailable user context.
    /// </exception>
    public string? Tenant { get; } = tenant;
}