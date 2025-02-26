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

using Carcass.Core;
using Carcass.SignalR.Hubs.Abstracts;
using Carcass.SignalR.Messages.Abstracts;
using Microsoft.AspNetCore.SignalR;

namespace Carcass.SignalR.Publishers.Abstracts;

/// <summary>
///     Represents an abstract base class for publishing messages to a SignalR hub.
/// </summary>
/// <typeparam name="THub">The type of the SignalR hub. Must derive from <see cref="AuthorizeHub" />.</typeparam>
/// <typeparam name="TMessage">The type of the message being sent. Must implement <see cref="IMessage" />.</typeparam>
public abstract class HubPublisher<THub, TMessage>
    where THub : AuthorizeHub
    where TMessage : IMessage
{
    /// <summary>
    ///     Represents the context for a SignalR hub, enabling communication between the server and clients connected to the
    ///     specified hub.
    ///     Provides functionalities to send messages, manage groups, and interact with clients connected to the hub.
    /// </summary>
    /// <typeparam name="THub">
    ///     The type of the SignalR hub associated with the context. It must inherit from <see cref="AuthorizeHub" />.
    /// </typeparam>
    // ReSharper disable once NotAccessedField.Global
    protected readonly IHubContext<THub> HubContext;

    /// <summary>
    ///     Represents a base class to facilitate sending messages to SignalR hubs with specific types of hubs and messages.
    /// </summary>
    /// <typeparam name="THub">The type of SignalR hub, derived from <see cref="AuthorizeHub" />.</typeparam>
    /// <typeparam name="TMessage">The type of message to be sent, implementing <see cref="IMessage" />.</typeparam>
    protected HubPublisher(IHubContext<THub> hubContext)
    {
        ArgumentVerifier.NotNull(hubContext, nameof(hubContext));

        HubContext = hubContext;
    }

    /// <summary>
    ///     Sends a message asynchronously to the connected SignalR hub clients.
    /// </summary>
    /// <param name="message">The message to send. Must implement the <see cref="IMessage" /> interface.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="message" /> parameter is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    public abstract Task SendAsync(TMessage message, CancellationToken cancellationToken = default);
}