using Carcass.Data.Core.EventSourcing.Aggregates.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.EventStoreDb.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using EventStore.Client;

namespace Carcass.Data.EventStoreDb.Aggregates.Extensions;

/// <summary>
///     Provides extension methods for aggregating and applying resolved events to an aggregate.
/// </summary>
public static class AggregateExtensions
{
    /// <summary>
    ///     Applies a collection of resolved events to an aggregate to reconstruct its state.
    /// </summary>
    /// <typeparam name="TAggregate">The type of the aggregate implementing <see cref="Aggregate" />.</typeparam>
    /// <param name="aggregate">The aggregate instance to which the events will be applied.</param>
    /// <param name="resolvedEvents">The collection of resolved events to apply.</param>
    /// <param name="domainEventLocator">
    ///     The locator used to identify and create domain event instances from resolved events.
    /// </param>
    /// <param name="domainEventUpgraderDispatcher">
    ///     The dispatcher responsible for upgrading domain events to the latest version.
    /// </param>
    /// <param name="jsonProvider">The provider for handling JSON serialization and deserialization.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="aggregate" /> or any of the parameters are null.</exception>
    /// <remarks>
    ///     This method reconstructs the aggregate's state by processing and applying each event in the provided collection.
    /// </remarks>
    public static void ApplyResolvedEvents<TAggregate>(
        this TAggregate aggregate,
        IList<ResolvedEvent> resolvedEvents,
        IDomainEventLocator domainEventLocator,
        IDomainEventUpgraderDispatcher domainEventUpgraderDispatcher,
        IJsonProvider jsonProvider
    ) where TAggregate : Aggregate, new()
    {
        List<IDomainEvent> history = [];
        foreach (ResolvedEvent resolvedEvent in resolvedEvents)
        {
            if (!resolvedEvent.TryGetDomainEvent(
                    out IDomainEvent? domainEvent,
                    domainEventLocator,
                    jsonProvider
                )
               ) continue;

            IDomainEvent upgradedDomainEvent = domainEventUpgraderDispatcher.DispatchDomainEvent(domainEvent!);
            history.Add(upgradedDomainEvent);
        }

        aggregate.Load(resolvedEvents.Last().Event.GetEventNumber(), history);
    }
}