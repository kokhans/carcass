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

namespace Carcass.Core.Accessors.TenantId.Abstracts;

/// <summary>
///     Provides an interface for accessing a tenant identifier, allowing retrieval
///     of the tenant ID if available.
/// </summary>
public interface ITenantIdAccessor
{
    /// <summary>
    ///     Attempts to retrieve the identifier of the current tenant.
    /// </summary>
    /// <returns>
    ///     A string representing the tenant identifier, or null if it cannot be determined.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the tenant identifier cannot be accessed or is invalid.
    /// </exception>
    string? TryGetTenantId();
}