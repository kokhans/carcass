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
using Carcass.SignalR.Dispatchers.Abstracts;
using Carcass.SignalR.Hubs.Abstracts;
using Carcass.SignalR.Messages.Abstracts;
using Carcass.SignalR.Publishers.Abstracts;
using Microsoft.Extensions.DependencyInjection;

namespace Carcass.SignalR.Dispatchers;

/// <summary>
///     A class that provides in-memory dispatching of messages to a specific SignalR hub.
/// </summary>
public sealed class InMemoryMessageDispatcher : IMessageDispatcher
{
    /// <summary>
    ///     Represents a factory to create IServiceScope instances for resolving scoped services.
    /// </summary>
    /// <remarks>
    ///     Used to create a scope that provides dependency injection for resolving services,
    ///     ensuring proper lifetime management of scoped dependencies during message dispatch.
    /// </remarks>
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    ///     Provides an in-memory implementation for dispatching messages to SignalR hubs.
    /// </summary>
    public InMemoryMessageDispatcher(IServiceScopeFactory serviceScopeFactory)
    {
        ArgumentVerifier.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));

        _serviceScopeFactory = serviceScopeFactory;
    }

    /// <summary>
    ///     Dispatches a message of a specified type to a SignalR hub of a specified type asynchronously.
    /// </summary>
    /// <typeparam name="THub">
    ///     The type of the SignalR hub to dispatch the message to. Must inherit from
    ///     <see cref="AuthorizeHub" />.
    /// </typeparam>
    /// <typeparam name="TMessage">The type of the message to dispatch. Must implement <see cref="IMessage" />.</typeparam>
    /// <param name="message">The message to be dispatched. Cannot be null.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous dispatch operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="message" /> is null.</exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown if the operation is canceled via
    ///     <paramref name="cancellationToken" />.
    /// </exception>
    public async Task DispatchAsync<THub, TMessage>(
        TMessage message,
        CancellationToken cancellationToken = default
    )
        where THub : AuthorizeHub
        where TMessage : class, IMessage
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentVerifier.NotNull(message, nameof(message));

        using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
        HubPublisher<THub, TMessage> hubPublisher = serviceScope.ServiceProvider
            .GetRequiredService<HubPublisher<THub, TMessage>>();

        await hubPublisher.SendAsync(message, cancellationToken);
    }
}