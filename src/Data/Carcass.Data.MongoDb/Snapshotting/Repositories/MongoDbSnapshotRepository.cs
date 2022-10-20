// MIT License
//
// Copyright (c) 2022 Serhii Kokhan
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
using Carcass.Data.Core.Snapshotting.Abstracts;
using Carcass.Data.Core.Snapshotting.Repositories.Abstracts;
using Carcass.Data.Core.Snapshotting.ResolutionStrategies.Abstracts;
using Carcass.Data.MongoDb.Sessions.Abstracts;

namespace Carcass.Data.MongoDb.Snapshotting.Repositories;

public sealed class MongoDbSnapshotRepository : ISnapshotRepository
{
    private readonly IMongoDbSession _mongoDbSession;
    private readonly ISnapshotNameResolutionStrategy _snapshotNameResolutionStrategy;

    public MongoDbSnapshotRepository(
        IMongoDbSession mongoDbSession,
        ISnapshotNameResolutionStrategy snapshotNameResolutionStrategy
    )
    {
        ArgumentVerifier.NotNull(mongoDbSession, nameof(mongoDbSession));
        ArgumentVerifier.NotNull(snapshotNameResolutionStrategy, nameof(snapshotNameResolutionStrategy));

        _mongoDbSession = mongoDbSession;
        _snapshotNameResolutionStrategy = snapshotNameResolutionStrategy;
    }

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

        DateTime timestamp = Clock.Current.UtcNow;
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