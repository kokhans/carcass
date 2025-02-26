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

using Carcass.Core.Accessors.CorrelationId.Abstracts;

namespace Carcass.Core.Accessors.CorrelationId;

/// <summary>
///     Provides a nullable implementation of <see cref="ICorrelationIdAccessor" />.
///     This class always returns null for the correlation ID, indicating no correlation ID is available.
/// </summary>
public sealed class NullableCorrelationIdAccessor : ICorrelationIdAccessor
{
    /// <summary>
    ///     Attempts to retrieve the current nullable correlation ID, which may be used for request tracking in distributed
    ///     systems.
    /// </summary>
    /// <returns>
    ///     The current correlation ID as a nullable string, or null if no correlation ID is available.
    /// </returns>
    public string? TryGetCorrelationId() => null;
}