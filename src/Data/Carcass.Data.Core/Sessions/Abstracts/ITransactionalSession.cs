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

namespace Carcass.Data.Core.Sessions.Abstracts;

/// <summary>
///     Defines a contract for a transactional session, enabling operations
///     for managing transactions, such as committing and rolling back.
/// </summary>
/// <typeparam name="TTransactionId">The type of the transaction identifier.</typeparam>
public interface ITransactionalSession<out TTransactionId> : ISession
{
    /// <summary>
    ///     Gets the unique identifier of the currently active transaction.
    /// </summary>
    /// <returns>
    ///     The transaction identifier, or <c>null</c> if no transaction is active.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if an attempt is made to retrieve the transaction identifier
    ///     when the session does not support transactions.
    /// </exception>
    TTransactionId? TransactionId { get; }

    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Commits the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> that can be used to observe while waiting for the operation to complete.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the transaction cannot be committed because it has not been started.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown when the operation is canceled via the provided <paramref name="cancellationToken" />.
    /// </exception>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Rolls back the current transaction if it is active and reverts any changes made in the transaction.
    /// </summary>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> that can be used to cancel the rollback operation.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if the transaction cannot be rolled back because it has not been started.
    /// </exception>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}