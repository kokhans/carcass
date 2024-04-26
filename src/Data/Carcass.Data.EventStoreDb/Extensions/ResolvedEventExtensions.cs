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

using System.Text;
using Carcass.Core;
using Carcass.Data.Core.EventSourcing.DomainEvents.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;
using Carcass.Json.Core.Providers.Abstracts;
using EventStore.Client;

namespace Carcass.Data.EventStoreDb.Extensions;

/// <summary>
///     Provides extension methods for working with <see cref="EventStore.Client.ResolvedEvent" /> instances.
/// </summary>
public static class ResolvedEventExtensions
{
    /// <summary>
    ///     Attempts to retrieve a domain event from the specified <see cref="ResolvedEvent" />.
    /// </summary>
    /// <param name="resolvedEvent">The resolved event containing the data to attempt to deserialize into a domain event.</param>
    /// <param name="domainEvent">
    ///     When this method returns, contains the domain event if the operation succeeded; otherwise, <c>null</c>.
    /// </param>
    /// <param name="domainEventLocator">The locator to determine the domain event type based on the event metadata.</param>
    /// <param name="jsonProvider">The JSON provider used to deserialize the event data into a domain event.</param>
    /// <returns>
    ///     <c>true</c> if a domain event could be successfully retrieved and deserialized; <c>false</c> otherwise.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="domainEventLocator" /> or <paramref name="jsonProvider" /> is <c>null</c>.
    /// </exception>
    public static bool TryGetDomainEvent(
        this ResolvedEvent resolvedEvent,
        out IDomainEvent? domainEvent,
        IDomainEventLocator domainEventLocator,
        IJsonProvider jsonProvider
    )
    {
        ArgumentVerifier.NotNull(domainEventLocator, nameof(domainEventLocator));
        ArgumentVerifier.NotNull(jsonProvider, nameof(jsonProvider));

        string data = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());
        Type? eventType = domainEventLocator.GetDomainEventType(resolvedEvent.Event.EventType);
        if (eventType is null)
        {
            domainEvent = null;

            return false;
        }

        domainEvent = jsonProvider.TryDeserialize(data, eventType) as IDomainEvent;

        return true;
    }
}