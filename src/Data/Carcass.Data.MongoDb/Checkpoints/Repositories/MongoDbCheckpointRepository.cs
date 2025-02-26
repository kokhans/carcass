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
using Carcass.Data.Core.EventSourcing.Checkpoints.Abstracts;
using Carcass.Data.Core.EventSourcing.Checkpoints.Repositories.Abstracts;
using Carcass.Data.MongoDb.Sessions.Abstracts;

namespace Carcass.Data.MongoDb.Checkpoints.Repositories;

/// <summary>
///     Repository for managing checkpoints in a MongoDB database.
///     Implements <see cref="ICheckpointRepository" /> to support the saving and loading of event stream checkpoints.
/// </summary>
public sealed class MongoDbCheckpointRepository : ICheckpointRepository
{
    /// <summary>
    ///     Represents the session used to interact with the MongoDB database for checkpoint storage and retrieval.
    /// </summary>
    /// <remarks>
    ///     This variable provides required operations for querying and manipulating data in MongoDB,
    ///     and is essential for ensuring proper checkpoint persistence.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the session is not initialized properly.
    /// </exception>
    private readonly IMongoDbSession _mongoDbSession;

    /// <summary>
    ///     A MongoDB-based implementation of the <see cref="ICheckpointRepository" /> interface for managing event stream
    ///     checkpoints.
    /// </summary>
    public MongoDbCheckpointRepository(IMongoDbSession mongoDbSession)
    {
        ArgumentVerifier.NotNull(mongoDbSession, nameof(mongoDbSession));

        _mongoDbSession = mongoDbSession;
    }

    /// <summary>
    ///     Loads a checkpoint from the MongoDB repository based on the specified stream and group names.
    /// </summary>
    /// <param name="streamName">The name of the stream associated with the checkpoint.</param>
    /// <param name="groupName">The name of the group associated with the checkpoint.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>
    ///     An <see cref="ICheckpoint" /> instance containing the checkpoint information if found; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="streamName" /> or <paramref name="groupName" /> is null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled through the <paramref name="cancellationToken" />.
    /// </exception>
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

    /// <summary>
    ///     Saves or updates the checkpoint for a specific stream and group in the MongoDB repository.
    /// </summary>
    /// <param name="streamName">The name of the stream associated with the checkpoint.</param>
    /// <param name="groupName">The name of the consumer group associated with the checkpoint.</param>
    /// <param name="position">The position in the stream to be saved as the checkpoint.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown if <paramref name="streamName" /> or <paramref name="groupName" /> is null,
    ///     empty, or consists only of white space.
    /// </exception>
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