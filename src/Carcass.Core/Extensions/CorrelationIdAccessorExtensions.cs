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

using Carcass.Core.Accessors.CorrelationId.Abstracts;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Carcass.Core.Extensions;

/// <summary>
///     Provides extension methods for the <see cref="ICorrelationIdAccessor" /> interface.
/// </summary>
public static class CorrelationIdAccessorExtensions
{
    /// <summary>
    ///     Retrieves the current correlation ID from the provided <see cref="ICorrelationIdAccessor" /> instance.
    /// </summary>
    /// <param name="correlationIdAccessor">The accessor instance used to obtain the correlation ID.</param>
    /// <returns>The current correlation ID as a string.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="correlationIdAccessor" /> is null.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the correlation ID is null, empty, or consists only of whitespace.
    /// </exception>
    public static string GetCorrelationId(this ICorrelationIdAccessor correlationIdAccessor)
    {
        ArgumentVerifier.NotNull(correlationIdAccessor, nameof(correlationIdAccessor));

        string? correlationId = correlationIdAccessor.TryGetCorrelationId();

        if (string.IsNullOrWhiteSpace(correlationId))
            throw new InvalidOperationException("CorrelationId is null.");

        return correlationId;
    }
}