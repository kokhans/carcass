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

using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Carcass.Core;
using Carcass.Metadata.Stores.Abstracts;

namespace Carcass.Metadata.Stores;

/// <summary>
///     Represents an in-memory implementation of the <see cref="IMetadataStore" /> interface.
///     Provides methods to perform CRUD operations on metadata stored in memory, allowing fast access.
/// </summary>
public sealed class InMemoryMetadataStore : IMetadataStore
{
    /// <summary>
    ///     Stores metadata in-memory using a thread-safe dictionary for concurrent processing.
    ///     This variable contains the metadata as key-value pairs,
    ///     where the key is a string and the value can be null or any object.
    /// </summary>
    /// <remarks>
    ///     The _metadata dictionary allows efficient and thread-safe retrieval,
    ///     addition, updating, and deletion of metadata entries.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if a null or invalid key is provided in operations where the key is required.
    /// </exception>
    private readonly ConcurrentDictionary<string, object?> _metadata = new();

    /// <summary>
    ///     Adds or updates a metadata entry in the store with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the metadata to add or update. Must not be null.</param>
    /// <param name="value">The value to associate with the specified key. Can be null.</param>
    /// <param name="cancellationToken">The cancellation token to observe for cancellation requests.</param>
    /// <returns>A task that represents the completion of the add or update operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="key" /> is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled through the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public Task AddOrUpdateMetadataAsync(string key, object? value, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(key, nameof(key));

        _metadata.AddOrUpdate(key, value, (_, _) => value);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Deletes metadata associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the metadata to be deleted.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided key is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled via the cancellation token.</exception>
    public Task DeleteMetadataAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(key, nameof(key));

        _metadata.TryRemove(key, out _);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Retrieves all metadata entries as a read-only dictionary.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A token to monitor for the operation cancellation request.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. On completion, the task contains a read-only dictionary of metadata
    ///     entries.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the provided cancellation token.
    /// </exception>
    public Task<ReadOnlyDictionary<string, object?>> GetMetadataAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new ReadOnlyDictionary<string, object?>(_metadata));
    }
}