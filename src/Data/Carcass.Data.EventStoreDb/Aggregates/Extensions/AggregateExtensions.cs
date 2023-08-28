using Carcass.Data.Core.EventSourcing.Aggregates.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Locators.Abstracts;
using Carcass.Data.Core.EventSourcing.DomainEvents.Upgraders.Abstracts;
using Carcass.Data.EventStoreDb.Extensions;
using Carcass.Json.Core.Providers.Abstracts;
using EventStore.Client;

namespace Carcass.Data.EventStoreDb.Aggregates.Extensions;

public static class AggregateExtensions
{
    public static void ApplyResolvedEvents<TAggregate>(
        this TAggregate aggregate,
        IList<ResolvedEvent> resolvedEvents,
        IDomainEventLocator domainEventLocator,
        IDomainEventUpgraderDispatcher domainEventUpgraderDispatcher,
        IJsonProvider jsonProvider
    ) where TAggregate : Aggregate, new()
    {
        List<IDomainEvent> history = new();
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