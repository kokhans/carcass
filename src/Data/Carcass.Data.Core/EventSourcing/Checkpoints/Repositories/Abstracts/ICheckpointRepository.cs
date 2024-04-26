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

using Carcass.Data.Core.EventSourcing.Checkpoints.Abstracts;

namespace Carcass.Data.Core.EventSourcing.Checkpoints.Repositories.Abstracts;

/// <summary>
///     Provides methods for managing checkpoints for event sourcing streams.
/// </summary>
public interface ICheckpointRepository
{
    /// <summary>
    ///     Loads a checkpoint associated with a specific stream and group from storage asynchronously.
    ///     The checkpoint tracks the committed position of the stream.
    /// </summary>
    /// <param name="streamName">The name of the stream whose checkpoint is to be loaded.</param>
    /// <param name="groupName">The group name associated with the checkpoint.</param>
    /// <param name="cancellationToken">
    ///     A token that can be used to request cancellation of the asynchronous operation.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation. The task result contains the loaded
    ///     checkpoint if it exists; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="streamName" /> or <paramref name="groupName" /> is null.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown when the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    Task<ICheckpoint?> LoadCheckpointAsync(
        string streamName,
        string groupName,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    ///     Saves the checkpoint with the specified stream name, group name, and position.
    /// </summary>
    /// <param name="streamName">The name of the event stream associated with the checkpoint.</param>
    /// <param name="groupName">The name of the consumer group associated with the checkpoint.</param>
    /// <param name="position">The position in the stream that represents the checkpoint.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="streamName" /> or <paramref name="groupName" /> is
    ///     null.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    Task SaveCheckpointAsync(
        string streamName,
        string groupName,
        long position,
        CancellationToken cancellationToken = default
    );
}