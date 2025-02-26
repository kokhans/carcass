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
using Carcass.Data.MongoDb.Entities.Abstracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

#pragma warning disable CS8618

namespace Carcass.Data.MongoDb.Snapshotting;

/// <summary>
///     Represents a snapshot document used in MongoDB for storing the state of an aggregate at a specific point in time.
///     Implements <c>IIdentifiableDocument</c> and <c>ISnapshot</c> for identification and snapshot functionalities.
/// </summary>
public sealed class SnapshotDocument : IIdentifiableDocument, ISnapshot
{
    /// <summary>
    ///     Gets or sets the unique identifier for the snapshot document.
    /// </summary>
    /// <value>
    ///     The unique identifier of type <see cref="ObjectId" /> used to identify this document.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the identifier is set to an invalid or improperly formatted <see cref="ObjectId" />.
    /// </exception>
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public ObjectId Id { get; set; }

    /// <summary>
    ///     Represents the unique key associated with the aggregate. This key is used to identify and manage a
    ///     specific snapshot instance across the repository and database operations.
    /// </summary>
    /// <remarks>
    ///     The <c>AggregateKey</c> is a crucial identifier for snapshots, ensuring uniqueness and consistency
    ///     in snapshotting operations, such as saving or loading a snapshot.
    /// </remarks>
    public string AggregateKey { get; set; }

    /// <summary>
    ///     Represents the schema version of the aggregate associated with the snapshot.
    /// </summary>
    /// <remarks>
    ///     This property is used to track the version of the aggregate's schema at the time the snapshot was created.
    ///     Updating this property ensures compatibility between aggregate schemas and their associated snapshots.
    /// </remarks>
    /// <returns>
    ///     A long integer value representing the schema version of the aggregate.
    /// </returns>
    public long AggregateSchemaVersion { get; set; }

    /// <summary>
    ///     Gets or sets the serialized data representing the state of the snapshot.
    /// </summary>
    /// <value>
    ///     A string containing the snapshot's serialized state, or null if no state is available.
    /// </value>
    public string? Payload { get; set; }

    /// <summary>
    ///     Gets or sets the timestamp indicating when the snapshot was created or last modified.
    /// </summary>
    /// <remarks>
    ///     The <see cref="Timestamp" /> property is used to record the exact date and time at which
    ///     a snapshot is taken or updated. It is represented using <see cref="DateTime" /> and is serialized
    ///     as a BSON document for MongoDB storage.
    /// </remarks>
    /// <value>
    ///     A <see cref="DateTime" /> representing the creation or modification time of the snapshot.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the property is set to an invalid or unsupported date and time.
    /// </exception>
    [BsonRepresentation(BsonType.Document)]
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     Gets or sets the number of events that must occur before a snapshot is taken.
    /// </summary>
    /// <remarks>
    ///     This property defines the threshold of event counts after which a snapshot is created
    ///     to optimize performance during event sourcing by reducing the number of events to replay.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the value set is negative, as the event count must be a non-negative number.
    /// </exception>
    public long TakeSnapshotAfterEventsCount { get; set; }
}