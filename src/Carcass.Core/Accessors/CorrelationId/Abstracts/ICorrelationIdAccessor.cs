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

namespace Carcass.Core.Accessors.CorrelationId.Abstracts;

/// <summary>
///     Defines a contract to provide access to a correlation ID, typically used for tracking requests across distributed
///     systems.
/// </summary>
public interface ICorrelationIdAccessor
{
    /// <summary>
    ///     Attempts to retrieve the correlation ID from the current context.
    /// </summary>
    /// <returns>
    ///     The correlation ID as a string, or null if no correlation ID is available.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the method is not implemented or an unexpected error occurs during retrieval.
    /// </exception>
    string? TryGetCorrelationId();
}