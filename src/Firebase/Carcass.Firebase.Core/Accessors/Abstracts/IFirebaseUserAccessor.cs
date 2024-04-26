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

using Carcass.Firebase.Core.Models;

namespace Carcass.Firebase.Core.Accessors.Abstracts;

/// <summary>
///     Provides an interface to retrieve the current Firebase user information.
/// </summary>
public interface IFirebaseUserAccessor
{
    // ReSharper disable once UnusedMemberInSuper.Global
    /// <summary>
    ///     Retrieves the Firebase user associated with the current context.
    /// </summary>
    /// <returns>
    ///     A <see cref="FirebaseUser" /> object representing the authenticated Firebase user, or <c>null</c>
    ///     if no user is authenticated.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the required context necessary to retrieve the user is unavailable or improperly configured.
    /// </exception>
    FirebaseUser? GetFirebaseUser();
}