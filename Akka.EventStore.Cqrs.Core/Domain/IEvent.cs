using System;
using Akka.Routing;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Akka.EventStore.Cqrs.Core
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        DateTime UtcDate { get; }
        int Version { get; }
    }

    public abstract class Event : IEvent, IConsistentHashable, IMessageContext
    {
        protected Event(Guid aggregateId, int version)
        {
            AggregateId = aggregateId;
            UtcDate = SystemClock.UtcNow;
            Version = version;
            Metadata = new Dictionary<string, string>();
        }

        protected Event(Guid aggregateId, int version, IMessageContext context)
        {
            AggregateId = aggregateId;
            UtcDate = SystemClock.UtcNow;
            Version = version;
            Metadata = context.Metadata;
        }

        public Guid AggregateId { get; private set; }

        public DateTime UtcDate { get; private set; }

        public int Version { get; private set; }

        [JsonIgnore]
        public Dictionary<string, string> Metadata { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AggregateId; }
        }
        
    }
}