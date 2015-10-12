using System;
using Akka.Routing;

namespace Akka.EventStore.Cqrs.Core
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        DateTime UtcDate { get; }
        int Version { get; }
    }

    public abstract class Event : IEvent, IConsistentHashable
    {
        protected Event(Guid aggregateId, int version)
        {
            AggregateId = aggregateId;
            UtcDate = SystemClock.UtcNow;
            Version = version;
        }

        public Guid AggregateId { get; private set; }

        public DateTime UtcDate { get; private set; }

        public int Version { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AggregateId; }
        }
    }
}