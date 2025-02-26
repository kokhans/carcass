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

namespace Carcass.Metadata.Stores.Abstracts;

/// <summary>
///     Defines the contract for a metadata store that provides methods to add, update, delete, and retrieve metadata.
/// </summary>
public interface IMetadataStore
{
    /// <summary>
    ///     Adds or updates a metadata entry in the store with the specified key and value.
    /// </summary>
    /// <param name="key">The key that identifies the metadata entry. Must not be null or empty.</param>
    /// <param name="value">The value to associate with the specified key. Can be null.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the key is null or empty.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the provided cancellation
    ///     token.
    /// </exception>
    Task AddOrUpdateMetadataAsync(string key, object? value, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously deletes the metadata associated with the specified key.
    /// </summary>
    /// <param name="key">The key identifying the metadata entry to be deleted. Must not be null.</param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to propagate a notification
    ///     that the operation should be canceled.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the specified key is null.</exception>
    Task DeleteMetadataAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves metadata as a read-only dictionary asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token that can be used to cancel the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The result contains a read-only dictionary of the metadata.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is cancelled through the provided cancellation token.
    /// </exception>
    Task<ReadOnlyDictionary<string, object?>> GetMetadataAsync(CancellationToken cancellationToken = default);
}