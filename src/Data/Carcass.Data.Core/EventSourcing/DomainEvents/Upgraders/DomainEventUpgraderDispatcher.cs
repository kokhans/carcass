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
using Carcass.Data.Core.EventSourcing.DomainEvents.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;

namespace Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders;

/// <summary>
///     The DomainEventUpgraderDispatcher is responsible for upgrading domain events
///     to their latest version using the appropriate upgraders provided by the
///     <see cref="IDomainEventUpgraderFactory" />.
/// </summary>
public sealed class DomainEventUpgraderDispatcher : IDomainEventUpgraderDispatcher
{
    /// <summary>
    ///     Represents a factory used to retrieve the appropriate domain event upgrader
    ///     for a given domain event type.
    /// </summary>
    /// <remarks>
    ///     The factory is responsible for obtaining implementations of <see cref="IDomainEventUpgrader" />
    ///     that can upgrade domain events to newer versions, ensuring compatibility and extensibility
    ///     in event-sourced systems.
    /// </remarks>
    private readonly IDomainEventUpgraderFactory _domainEventUpgraderFactory;

    /// <summary>
    ///     This class is responsible for dispatching and upgrading domain events using the appropriate upgrader
    ///     provided by the <see cref="IDomainEventUpgraderFactory" />.
    /// </summary>
    public DomainEventUpgraderDispatcher(IDomainEventUpgraderFactory domainEventUpgraderFactory)
    {
        ArgumentVerifier.NotNull(domainEventUpgraderFactory, nameof(domainEventUpgraderFactory));

        _domainEventUpgraderFactory = domainEventUpgraderFactory;
    }

    /// <summary>
    ///     Dispatches a domain event to its corresponding upgrader, if one exists, and returns the upgraded domain event.
    /// </summary>
    /// <param name="domainEvent">
    ///     The domain event to be dispatched and possibly upgraded. Cannot be null.
    /// </param>
    /// <returns>
    ///     The upgraded domain event. If no upgrader is found, the original domain event is returned.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <paramref name="domainEvent" /> is null.
    /// </exception>
    public IDomainEvent DispatchDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentVerifier.NotNull(domainEvent, nameof(domainEvent));

        IDomainEvent upgradedDomainEvent = domainEvent;
        IDomainEventUpgrader? domainEventUpgrader;
        do
        {
            domainEventUpgrader = _domainEventUpgraderFactory.GetDomainEventUpgrader(upgradedDomainEvent.GetType());
            if (domainEventUpgrader is not null)
                upgradedDomainEvent = domainEventUpgrader.UpgradeDomainEvent(upgradedDomainEvent);
        } while (domainEventUpgrader is not null);

        return upgradedDomainEvent;
    }
}