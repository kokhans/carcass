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

using Carcass.Data.Core.EventSourcing.Aggregates.Abstracts;

// ReSharper disable UnusedMember.Global

namespace Carcass.Data.Core.EventSourcing.Aggregates.Repositories.Abstracts;

/// <summary>
///     Represents a repository for handling event-sourced aggregates, allowing
///     loading and saving of aggregates by interacting with the underlying event store.
/// </summary>
public interface IAggregateRepository
{
    /// <summary>
    ///     Loads an instance of an aggregate of the specified type from the event store.
    /// </summary>
    /// <typeparam name="TAggregate">
    ///     The type of the aggregate to load. Must inherit from <see cref="Aggregate" /> and have a
    ///     parameterless constructor.
    /// </typeparam>
    /// <param name="aggregateId">The unique identifier of the aggregate to load.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The reconstructed aggregate of type <typeparamref name="TAggregate" /> based on the stored event history.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="aggregateId" /> has a default value.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    Task<TAggregate> LoadAggregateAsync<TAggregate>(
        Guid aggregateId,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new();

    /// <summary>
    ///     Persists the specified aggregate and its associated events to the underlying event store.
    /// </summary>
    /// <typeparam name="TAggregate">
    ///     The type of the aggregate to be saved. It must derive from <see cref="Aggregate" /> and have a parameterless
    ///     constructor.
    /// </typeparam>
    /// <param name="aggregate">
    ///     The aggregate instance to save, containing its new and existing events.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token to observe for cancellation requests.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous save operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="aggregate" /> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if there is an error while appending the aggregate’s events to the event store.
    /// </exception>
    /// <exception cref="TaskCanceledException">
    ///     Thrown if the operation is canceled via the <paramref name="cancellationToken" />.
    /// </exception>
    Task SaveAggregateAsync<TAggregate>(
        TAggregate aggregate,
        CancellationToken cancellationToken = default
    ) where TAggregate : Aggregate, new();
}