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

// ReSharper disable UnusedMemberInSuper.Global

namespace Carcass.Data.Core.EventSourcing.Snapshotting.Abstracts;

/// <summary>
///     Represents the snapshot of an aggregate, containing essential data required to restore
///     the state of an aggregate in event sourcing.
/// </summary>
public interface ISnapshot
{
    /// <summary>
    ///     Gets or sets the unique identifier for the aggregate associated with the snapshot.
    /// </summary>
    /// <value>
    ///     A string representing the key that uniquely identifies the aggregate.
    /// </value>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when attempting to set the property to a null or empty string.
    /// </exception>
    string AggregateKey { get; set; }

    /// <summary>
    ///     Represents the version of the aggregate's schema associated with a snapshot.
    /// </summary>
    /// <remarks>
    ///     This property is used to identify compatibility between the snapshot's schema and the current state
    ///     of the aggregate. This ensures that snapshots can be correctly applied against the intended version
    ///     of an aggregate schema.
    /// </remarks>
    /// <value>
    ///     The version of the schema for the aggregate represented in the snapshot as a <see cref="long" />.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown during aggregate retrieval if the aggregate cannot be restored from the snapshot due to mismatch
    ///     in schema version or related issues.
    /// </exception>
    long AggregateSchemaVersion { get; set; }

    /// <summary>
    ///     Represents the serialized state of a snapshot in string format.
    /// </summary>
    /// <remarks>
    ///     This property is used to store and retrieve the data associated with a snapshot.
    ///     The value is typically expected to be a JSON string or another serialized format
    ///     that represents the state of an entity at a specific point in time.
    /// </remarks>
    /// <value>
    ///     A string representing the serialized snapshot, or null if no snapshot data is available.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an invalid or corrupted payload is encountered during deserialization.
    /// </exception>
    string? Payload { get; set; }

    /// <summary>
    ///     Represents the date and time when the snapshot was created.
    /// </summary>
    /// <remarks>
    ///     The timestamp is used to record when the snapshot was taken for tracking and historical purposes.
    /// </remarks>
    /// <value>
    ///     A <see cref="DateTime" /> value representing the creation date and time of the snapshot.
    /// </value>
    /// <exception cref="System.InvalidOperationException">
    ///     Thrown if the value set does not adhere to the expected input format or range.
    /// </exception>
    DateTime Timestamp { get; set; }

    /// <summary>
    ///     Gets or sets the number of events after which a snapshot is taken.
    /// </summary>
    /// <value>
    ///     The count of events to process before creating a snapshot.
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the value set is less than or equal to zero as snapshots must be managed after a positive number of
    ///     events.
    /// </exception>
    long TakeSnapshotAfterEventsCount { get; set; }
}