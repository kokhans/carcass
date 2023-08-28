// MIT License
//
// Copyright (c) 2022-2023 Serhii Kokhan
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
using Carcass.Data.Core.EventSourcing.Checkpoints.Abstracts;
using Carcass.Data.Core.EventSourcing.Checkpoints.Repositories.Abstracts;
using Carcass.Data.MongoDb.Sessions.Abstracts;

namespace Carcass.Data.MongoDb.Checkpoints.Repositories;

public sealed class MongoDbCheckpointRepository : ICheckpointRepository
{
    private readonly IMongoDbSession _mongoDbSession;

    public MongoDbCheckpointRepository(IMongoDbSession mongoDbSession)
    {
        ArgumentVerifier.NotNull(mongoDbSession, nameof(mongoDbSession));

        _mongoDbSession = mongoDbSession;
    }

    public async Task<ICheckpoint?> LoadCheckpointAsync(
        string streamName,
        string groupName,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(streamName, streamName);
        ArgumentVerifier.NotNull(groupName, groupName);

        IList<CheckpointDocument> checkpoints = await _mongoDbSession.QueryAsync<CheckpointDocument>(
            cd => cd.StreamName.Equals(streamName, StringComparison.InvariantCultureIgnoreCase) &&
                  cd.GroupName.Equals(groupName, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken
        );

        return checkpoints.SingleOrDefault();
    }

    public async Task SaveCheckpointAsync(
        string streamName,
        string groupName,
        long position,
        CancellationToken cancellationToken = default
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(streamName, streamName);
        ArgumentVerifier.NotNull(groupName, groupName);

        IList<CheckpointDocument> checkpoints = await _mongoDbSession.QueryAsync<CheckpointDocument>(
            cd => cd.StreamName.Equals(streamName, StringComparison.InvariantCultureIgnoreCase) &&
                  cd.GroupName.Equals(groupName, StringComparison.InvariantCultureIgnoreCase),
            cancellationToken
        );

        CheckpointDocument? checkpointDocument = checkpoints.SingleOrDefault();

        if (checkpointDocument is null)
        {
            checkpointDocument = new CheckpointDocument
            {
                StreamName = streamName,
                GroupName = groupName,
                CommittedPosition = position
            };

            await _mongoDbSession.CreateAsync(checkpointDocument, cancellationToken);
            return;
        }

        checkpointDocument.CommittedPosition = position;

        await _mongoDbSession.UpdateAsync(checkpointDocument, cancellationToken);
    }
}