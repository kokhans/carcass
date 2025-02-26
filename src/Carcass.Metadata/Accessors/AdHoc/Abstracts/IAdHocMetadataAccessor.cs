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

using System.Collections.ObjectModel;

// ReSharper disable UnusedMember.Global

namespace Carcass.Metadata.Accessors.AdHoc.Abstracts;

/// <summary>
///     Represents an interface for managing and accessing ad-hoc metadata.
///     Provides functionality for adding metadata dynamically and retrieving it asynchronously.
/// </summary>
public interface IAdHocMetadataAccessor
{
    /// <summary>
    ///     Adds or updates ad-hoc metadata with the specified key and value.
    /// </summary>
    /// <param name="key">The key associated with the metadata. Must not be null or empty.</param>
    /// <param name="value">The value of the metadata. Can be null.</param>
    /// <returns>The current instance of <see cref="IAdHocMetadataAccessor" /> to allow method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="key" /> is null or empty.</exception>
    IAdHocMetadataAccessor WithAdHocMetadata(string key, object? value);

    /// <summary>
    ///     Retrieves a combined set of metadata, including both persisted metadata and optionally ad-hoc metadata.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to monitor for cancellation requests while retrieving metadata.
    /// </param>
    /// <returns>
    ///     A read-only dictionary containing the combined metadata values.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the ad-hoc metadata contains duplicate keys, and the resolution strategy is set to throw an exception.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided <paramref name="cancellationToken" />.
    /// </exception>
    Task<ReadOnlyDictionary<string, object?>> GetMetadataAsync(CancellationToken cancellationToken = default);
}