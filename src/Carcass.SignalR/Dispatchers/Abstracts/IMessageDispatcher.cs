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

using Carcass.SignalR.Hubs.Abstracts;
using Carcass.SignalR.Messages.Abstracts;

// ReSharper disable UnusedMember.Global

namespace Carcass.SignalR.Dispatchers.Abstracts;

/// <summary>
///     Represents a contract for dispatching messages to a specific SignalR hub.
/// </summary>
public interface IMessageDispatcher
{
    /// <summary>
    ///     Dispatches a message to a specified SignalR hub asynchronously.
    /// </summary>
    /// <typeparam name="THub">
    ///     The type of the SignalR hub that the message will be dispatched to. Must inherit from
    ///     <see cref="AuthorizeHub" />.
    /// </typeparam>
    /// <typeparam name="TMessage">The type of the message to dispatch. Must implement the <see cref="IMessage" /> interface.</typeparam>
    /// <param name="message">The message to be dispatched. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation of dispatching the message.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="message" /> parameter is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via the
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    Task DispatchAsync<THub, TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default
    )
        where THub : AuthorizeHub
        where TMessage : class, IMessage;
}