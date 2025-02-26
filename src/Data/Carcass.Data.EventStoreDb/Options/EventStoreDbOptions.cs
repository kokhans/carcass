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

using System.ComponentModel.DataAnnotations;

namespace Carcass.Data.EventStoreDb.Options;

/// <summary>
///     Represents the configuration options for connecting to an EventStore database,
///     including parameters for snapshots and event limits.
/// </summary>
public sealed class EventStoreDbOptions
{
    /// <summary>
    ///     Represents the connection string required to connect to the Event Store database.
    /// </summary>
    /// <remarks>
    ///     This property is mandatory and should be configured with the appropriate connection string
    ///     to establish communication with the target Event Store database.
    /// </remarks>
    /// <exception cref="ValidationException">Thrown if the value is null or not provided.</exception>
    [Required]
    public required string ConnectionString { get; init; }

    /// <summary>
    ///     Specifies the number of events after which a snapshot of an aggregate should be taken.
    /// </summary>
    /// <value>
    ///     A positive integer representing the event threshold for creating a snapshot.
    /// </value>
    /// <remarks>
    ///     Snapshots can improve performance by reducing the number of events that need to be loaded to
    ///     reconstruct an aggregate. When the count of events for an aggregate exceeds this value,
    ///     snapshotting logic determines whether a snapshot should be created or updated.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if the value set for this property is less than or equal to zero.
    /// </exception>
    [Required]
    public long TakeSnapshotAfterEventsCount { get; init; } = 900;

    /// <summary>
    ///     Specifies the maximum number of events that can be associated with a stream in the Event Store.
    ///     Used to enforce a limit when managing event streams and snapshots.
    /// </summary>
    /// <remarks>
    ///     This property is typically utilized to define the maximum size or lifespan of event streams
    ///     before adjustments, such as snapshot creation or archiving, are applied.
    /// </remarks>
    /// <returns>
    ///     A <see cref="long" /> representing the maximum count of events allowed in a single event stream.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when operations on streams exceed this configured maximum.
    /// </exception>
    [Required]
    public long EventsMaxCount { get; init; } = 4096;
}