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
using Carcass.Data.MongoDb.Entities.Abstracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#pragma warning disable CS8618

namespace Carcass.Data.MongoDb.Checkpoints;

/// <summary>
///     Represents a MongoDB document that tracks the state of a stream
///     and its committed position for a specific consumer group.
/// </summary>
/// <remarks>
///     This class serves as a persistent checkpoint mechanism for event sourcing systems,
///     implementing both <see cref="IIdentifiableDocument" /> and <see cref="ICheckpoint" /> interfaces to provide unique
///     identification and checkpoint functionalities.
/// </remarks>
public sealed class CheckpointDocument : IIdentifiableDocument, ICheckpoint
{
    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    /// <summary>
    ///     Gets or sets the name of the consumer group associated with a checkpoint.
    /// </summary>
    /// <value>
    ///     The name of the group to which the checkpoint belongs.
    /// </value>
    /// <exception cref="System.ArgumentNullException">
    ///     Thrown when attempting to set a null or empty value.
    /// </exception>
    public string GroupName { get; set; }

    /// <summary>
    ///     Gets or sets the name of the stream associated with the checkpoint.
    /// </summary>
    /// <value>
    ///     A string representing the identifier of the stream.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when attempting to set the stream name to null.
    /// </exception>
    public string StreamName { get; set; }

    /// <summary>
    ///     Gets or sets the position in the event stream that has been successfully processed and committed.
    /// </summary>
    /// <value>
    ///     The position in the event stream, represented as a <see cref="long" /> value.
    /// </value>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if this property is accessed inappropriately, such as when an invalid state is encountered.
    /// </exception>
    /// <remarks>
    ///     This property is used within checkpointing mechanisms to track progress of processing in an event stream.
    ///     It ensures idempotency and fault tolerance.
    /// </remarks>
    public long CommittedPosition { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier for the checkpoint document.
    /// </summary>
    /// <value>
    ///     An <see cref="ObjectId" /> that uniquely identifies the checkpoint document in the database.
    /// </value>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the identifier is not properly initialized or conflicts with an existing identifier.
    /// </exception>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; set; }
}