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

using Carcass.Core;
using Carcass.Data.Core.EventSourcing.Snapshotting.Abstracts;
using Carcass.Data.Core.EventSourcing.Snapshotting.Repositories.Abstracts;
using Carcass.Data.Core.EventSourcing.Snapshotting.ResolutionStrategies.Abstracts;
using Carcass.Data.MongoDb.Sessions.Abstracts;

namespace Carcass.Data.MongoDb.Snapshotting.Repositories;

/// <summary>
///     A repository implementation for managing snapshots in a MongoDB database.
///     This class is responsible for saving and loading snapshot data associated
///     with an aggregate using MongoDB as the storage backend.
/// </summary>
public sealed class MongoDbSnapshotRepository : ISnapshotRepository
{
    /// <summary>
    ///     Represents the MongoDB session used for interacting with the MongoDB datastore.
    /// </summary>
    /// <remarks>
    ///     This variable provides access to the abstraction of MongoDB operations
    ///     required for querying, creating, and updating snapshot-related data.
    ///     It is a dependency injected into the repository for accessing the database.
    /// </remarks>
    private readonly IMongoDbSession _mongoDbSession;

    /// <summary>
    ///     Represents a strategy for resolving the names of snapshot collections used in the MongoDb snapshot repository.
    /// </summary>
    private readonly ISnapshotNameResolutionStrategy _snapshotNameResolutionStrategy;

    /// <summary>
    ///     Provides the current time for the repository, offering a centralized way to retrieve
    ///     the current UTC time during snapshot operations.
    /// </summary>
    private readonly TimeProvider _timeProvider;

    /// <summary>
    ///     Provides a MongoDB-based implementation for saving and loading snapshots in an event-sourcing system.
    /// </summary>
    public MongoDbSnapshotRepository(
        TimeProvider timeProvider,
        IMongoDbSession mongoDbSession,
        ISnapshotNameResolutionStrategy snapshotNameResolutionStrategy
    )
    {
        ArgumentVerifier.NotNull(timeProvider, nameof(timeProvider));
        ArgumentVerifier.NotNull(mongoDbSession, nameof(mongoDbSession));
        ArgumentVerifier.NotNull(snapshotNameResolutionStrategy, nameof(snapshotNameResolutionStrategy));

        _timeProvider = timeProvider;
        _mongoDbSession = mongoDbSession;
        _snapshotNameResolutionStrategy = snapshotNameResolutionStrategy;
    }

    /// <summary>
    ///     Saves a snapshot for the specified aggregate, including metadata such as schema version, payload,
    ///     and the designated number of events required before a snapshot is taken.
    ///     If a snapshot already exists for the given aggregate, it will be updated; otherwise, a new snapshot will be
    ///     created.
    /// </summary>
    /// <param name="aggregateKey">The unique key identifying the aggregate for which the snapshot is being saved.</param>
    /// <param name="aggregateSchemaVersion">The version of the aggregate schema associated with the snapshot.</param>
    /// <param name="payload">The serialized state of the aggregate, which can be null if no payload is provided.</param>
    /// <param name="takeSnapshotAfterEventsCount">The number of events required before another snapshot is taken.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the operation to complete.</param>
    /// <returns>A task representing the asynchronous operation of saving the snapshot.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="aggregateKey" /> is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public async Task SaveSnapshotAsync(
        string aggregateKey,
        long aggregateSchemaVersion,
        string? payload,
        long takeSnapshotAfterEventsCount,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(aggregateKey, nameof(aggregateKey));

        DateTime timestamp = _timeProvider.GetUtcNow().UtcDateTime;
        string snapshotCollectionName = _snapshotNameResolutionStrategy.GetSnapshotName(aggregateKey);
        IList<SnapshotDocument> snapshots = await _mongoDbSession.QueryAsync<SnapshotDocument>(
            snapshotCollectionName,
            sd => sd.AggregateKey.Equals(aggregateKey, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken
        );
        SnapshotDocument? snapshotDocument = snapshots.SingleOrDefault();
        if (snapshotDocument is null)
        {
            snapshotDocument = new SnapshotDocument
            {
                AggregateKey = aggregateKey,
                AggregateSchemaVersion = aggregateSchemaVersion,
                Payload = payload,
                Timestamp = timestamp,
                TakeSnapshotAfterEventsCount = takeSnapshotAfterEventsCount
            };
            await _mongoDbSession.CreateAsync(snapshotCollectionName, snapshotDocument, cancellationToken);

            return;
        }

        snapshotDocument.AggregateSchemaVersion = aggregateSchemaVersion;
        snapshotDocument.Payload = payload;
        snapshotDocument.Timestamp = timestamp;
        snapshotDocument.TakeSnapshotAfterEventsCount = takeSnapshotAfterEventsCount;

        await _mongoDbSession.UpdateAsync(snapshotCollectionName, snapshotDocument, cancellationToken);
    }

    /// <summary>
    ///     Asynchronously loads a snapshot associated with the specified aggregate key.
    /// </summary>
    /// <param name="aggregateKey">The unique key identifying the aggregate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The snapshot corresponding to the specified aggregate key, or null if no snapshot is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="aggregateKey" /> parameter is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public async Task<ISnapshot?> LoadSnapshotAsync(string aggregateKey, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(aggregateKey, nameof(aggregateKey));

        IList<SnapshotDocument> snapshots = await _mongoDbSession.QueryAsync<SnapshotDocument>(
            _snapshotNameResolutionStrategy.GetSnapshotName(aggregateKey),
            sd => sd.AggregateKey.Equals(aggregateKey, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken
        );

        return snapshots.SingleOrDefault();
    }
}