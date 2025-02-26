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

using Carcass.Data.Core.EventSourcing.Snapshotting.Abstracts;

namespace Carcass.Data.Core.EventSourcing.Snapshotting.Repositories.Abstracts;

/// <summary>
///     Represents a repository interface for managing snapshots in an event-sourced system.
///     Provides methods for loading and saving aggregate snapshots.
/// </summary>
public interface ISnapshotRepository
{
    /// <summary>
    ///     Asynchronously loads a snapshot for a given aggregate key.
    /// </summary>
    /// <param name="aggregateKey">The unique key identifying the aggregate for which the snapshot is being retrieved.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the loaded
    ///     snapshot or null if no snapshot exists for the specified aggregate key.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="aggregateKey" /> is null or empty.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    Task<ISnapshot?> LoadSnapshotAsync(string aggregateKey, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Persists a snapshot of the aggregate state along with associated metadata into the storage to
    ///     optimize event replay performance.
    /// </summary>
    /// <param name="aggregateKey">The unique identifier of the aggregate for which the snapshot is being saved.</param>
    /// <param name="aggregateSchemaVersion">
    ///     The version of the schema for the aggregate being saved, ensuring consistency with
    ///     the snapshot data.
    /// </param>
    /// <param name="payload">
    ///     A serialized representation of the aggregate state to be stored as a snapshot. Can be null if no
    ///     state exists.
    /// </param>
    /// <param name="takeSnapshotAfterEventsCount">
    ///     The number of events after which a new snapshot should be taken for
    ///     performance optimization.
    /// </param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation of saving the snapshot.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="aggregateKey" /> is null or empty.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the snapshot data is invalid or mismatched with the schema
    ///     version.
    /// </exception>
    /// <remarks>
    ///     This method is typically called to enable faster reconstruction of aggregates by reducing the number of events
    ///     to replay.
    /// </remarks>
    Task SaveSnapshotAsync(
        string aggregateKey,
        long aggregateSchemaVersion,
        string? payload,
        long takeSnapshotAfterEventsCount,
        CancellationToken cancellationToken = default
    );
}