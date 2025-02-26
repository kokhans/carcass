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

using System.Data;

namespace Carcass.Data.Core.Sessions.Abstracts;

/// <summary>
///     Represents a transactional session within a relational database, providing transaction management
///     capabilities such as beginning, committing, and rolling back transactions.
/// </summary>
/// <typeparam name="TTransactionId">The type of the transaction identifier.</typeparam>
public interface IRelationDatabaseTransactionalSession<out TTransactionId> : ITransactionalSession<TTransactionId>
{
    // ReSharper disable once UnusedMember.Global
    /// <summary>
    ///     Begins a new database transaction asynchronously using the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">
    ///     The isolation level for the transaction. Defaults to
    ///     <see cref="System.Data.IsolationLevel.ReadCommitted" />.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token to observe while waiting for the task to complete. Defaults to
    ///     <see cref="System.Threading.CancellationToken.None" />.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the transaction cannot be started because the database context is null.
    /// </exception>
    /// <exception cref="System.OperationCanceledException">
    ///     Thrown if the operation is canceled via the cancellation token.
    /// </exception>
    Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default
    );
}