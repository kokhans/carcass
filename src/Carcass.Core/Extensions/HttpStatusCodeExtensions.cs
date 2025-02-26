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

using System.Net;

// ReSharper disable UnusedMember.Global

namespace Carcass.Core.Extensions;

/// <summary>
///     Provides extension methods for determining the category of HTTP status codes.
/// </summary>
public static class HttpStatusCodeExtensions
{
    /// <summary>
    ///     Determines whether the specified HTTP status code represents a successful status code (2xx).
    /// </summary>
    /// <param name="value">The HTTP status code to evaluate.</param>
    /// <returns>
    ///     True if the status code indicates success (between 200 and 299 inclusive); otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the provided HTTP status code value is invalid.
    /// </exception>
    public static bool IsSuccessHttpStatusCode(this HttpStatusCode value) =>
        (int) value >= 200 && (int) value <= 299;

    /// <summary>
    ///     Determines whether the specified HTTP status code represents a client error (4xx).
    /// </summary>
    /// <param name="value">The HTTP status code to evaluate.</param>
    /// <returns>
    ///     True if the status code indicates a client error (between 400 and 499 inclusive); otherwise, false.
    /// </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown if the provided HTTP status code value is invalid.
    /// </exception>
    public static bool IsClientErrorsHttpStatusCode(this HttpStatusCode value) =>
        (int) value >= 400 && (int) value <= 499;

    /// <summary>
    ///     Determines whether a given HTTP status code represents a server error (5xx).
    /// </summary>
    /// <param name="value">The HTTP status code to evaluate.</param>
    /// <returns>True if the status code is in the range of server errors (500–599), otherwise false.</returns>
    /// <exception cref="InvalidCastException">Thrown if the status code cannot be cast to an integer value for evaluation.</exception>
    public static bool IsServerErrorsHttpStatusCode(this HttpStatusCode value) =>
        (int) value >= 500 && (int) value <= 599;
}