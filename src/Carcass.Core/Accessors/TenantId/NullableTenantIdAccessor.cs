﻿// MIT License
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

using Carcass.Core.Accessors.TenantId.Abstracts;

namespace Carcass.Core.Accessors.TenantId;

/// <summary>
///     Represents an implementation of <see cref="ITenantIdAccessor" /> that always returns a null tenant identifier.
/// </summary>
/// <remarks>
///     This class is used in scenarios where no tenant context is available or required.
/// </remarks>
public sealed class NullableTenantIdAccessor : ITenantIdAccessor
{
    /// <summary>
    ///     Attempts to retrieve the tenant identifier.
    /// </summary>
    /// <returns>
    ///     The tenant ID as a nullable string, or null if no tenant ID is available.
    /// </returns>
    public string? TryGetTenantId() => null;
}