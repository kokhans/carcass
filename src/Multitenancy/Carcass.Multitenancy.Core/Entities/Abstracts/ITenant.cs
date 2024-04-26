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

namespace Carcass.Multitenancy.Core.Entities.Abstracts;

/// <summary>
///     Represents the abstraction for a tenant entity in a multitenant application.
/// </summary>
public interface ITenant
{
    /// <summary>
    ///     Gets the unique identifier of the tenant.
    /// </summary>
    /// <returns>A string representing the unique identifier.</returns>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the identifier cannot be retrieved due to the tenant being uninitialized or in an invalid state.
    /// </exception>
    string Id { get; }

    /// <summary>
    ///     Represents a unique identifier for a tenant within a multitenant application.
    /// </summary>
    /// <value>
    ///     The unique string that identifies the tenant. This property is read-only.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the identifier cannot be resolved or accessed for the tenant.
    /// </exception>
    string Identifier { get; }

    /// <summary>
    ///     Gets or sets a collection of key-value pairs associated with the tenant.
    /// </summary>
    /// <remarks>
    ///     This property is used to store additional data or metadata related to the tenant.
    /// </remarks>
    /// <value>
    ///     A dictionary containing items where the key is a string and the value is an object.
    /// </value>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown when setting the dictionary to null.
    /// </exception>
    Dictionary<string, object> Items { get; }
}