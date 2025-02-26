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

using System.Collections.ObjectModel;

// ReSharper disable MemberCanBePrivate.Global

namespace Carcass.Data.Core.EventSourcing.Aggregates.Abstracts;

/// <summary>
///     Represents the base class for implementing event-sourced aggregates.
///     Provides functionality for managing aggregate identity, event application, versioning, and event history.
/// </summary>
public abstract class Aggregate
{
    /// <summary>
    ///     A private internal list that maintains a history of events associated with the aggregate.
    /// </summary>
    /// <remarks>
    ///     This list is used to keep track of all events applied to an aggregate instance, providing
    ///     a complete record of state changes. It is not directly accessible outside the aggregate
    ///     and is exposed as a read-only collection through the <see cref="History" /> property.
    /// </remarks>
    private readonly List<object> _history = [];

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    /// <summary>
    ///     Gets or sets the unique identifier of the aggregate.
    /// </summary>
    /// <value>
    ///     A globally unique identifier (GUID) that identifies the aggregate instance.
    /// </value>
    public Guid Id { get; set; }

    /// <summary>
    ///     Represents the version of the aggregate, indicating the state of the aggregate in terms of event sequence.
    /// </summary>
    /// <remarks>
    ///     This property is used to track the current version of the aggregate, which reflects the number of changes (events)
    ///     that have been applied to it. This value is critical for maintaining concurrency and ensuring consistency when
    ///     persisting changes.
    /// </remarks>
    /// <value>
    ///     A <see cref="long" /> value representing the version of the aggregate.
    ///     The initial value is set to -1, indicating that the aggregate has not been initialized with any events.
    /// </value>
    /// <exception cref="InvalidOperationException">
    ///     Thrown during certain operations if the version information is not set correctly or is inconsistent with the event
    ///     stream.
    /// </exception>
    public long Version { get; set; } = -1;

    /// <summary>
    ///     Provides a read-only collection of historical events applied to the aggregate.
    /// </summary>
    /// <returns>
    ///     A <see cref="ReadOnlyCollection{T}" /> representing the history of events associated with the aggregate.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if history access encounters an invalid state.
    /// </exception>
    public ReadOnlyCollection<object> History => _history.AsReadOnly();

    /// <summary>
    ///     Handles the application of a single event to modify the state of the aggregate.
    ///     This method needs to be overridden by derived classes to specify the behavior for specific events.
    /// </summary>
    /// <param name="event">
    ///     The event object that carries the information to update the state of the aggregate.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="event" /> parameter is null.
    /// </exception>
    /// <remarks>
    ///     The derived implementation should contain logic to update the state of the aggregate
    ///     according to the specific rules and behavior defined for the event type.
    /// </remarks>
    protected abstract void When(object @event);

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Applies a given event to the aggregate, invoking the corresponding handler
    ///     and recording the event in the aggregate's history.
    /// </summary>
    /// <param name="event">The event to apply to the aggregate.</param>
    /// <exception cref="ArgumentNullException">Thrown if the provided event is null.</exception>
    public void Apply(object @event)
    {
        When(@event);

        _history.Add(@event);
    }

    /// <summary>
    ///     Loads historical events into the aggregate, restoring its state to reflect the given version and event history.
    /// </summary>
    /// <param name="version">
    ///     The version of the aggregate to set after applying the history.
    /// </param>
    /// <param name="history">
    ///     A collection of events to apply to the aggregate to restore its state.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the provided history collection is null.
    /// </exception>
    public void Load(long version, IEnumerable<object> history)
    {
        Version = version;

        foreach (object @event in history)
            When(@event);
    }
}