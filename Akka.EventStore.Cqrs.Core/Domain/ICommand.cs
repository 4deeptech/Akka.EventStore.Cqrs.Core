using Akka.Routing;
using System;
using System.Collections.Generic;

namespace Akka.EventStore.Cqrs.Core
{
    public interface ICommand 
    {
        Guid AggregateId { get; }
        Dictionary<string, string> Metadata { get; }
    }

    public abstract class Command : ICommand, IMessageContext, IConsistentHashable
    {
        protected Command(Guid aggregateId)
        {
            AggregateId = aggregateId;
            UtcDate = SystemClock.UtcNow;
            Version = -1;
            Metadata = new Dictionary<string, string>();
        }
        protected Command(Guid aggregateId, int version)
        {
            AggregateId = aggregateId;
            UtcDate = SystemClock.UtcNow;
            Version = version;
            Metadata = new Dictionary<string, string>();
        }

        protected Command(Guid aggregateId, IMessageContext context)
        {
            AggregateId = aggregateId;
            UtcDate = SystemClock.UtcNow;
            Version = -1;
            Metadata = context.Metadata;
        }

        protected Command(Guid aggregateId, int version, IMessageContext context)
        {
            AggregateId = aggregateId;
            UtcDate = SystemClock.UtcNow;
            Version = version;
            Metadata = context.Metadata;
        }

        public Guid AggregateId { get; private set; }

        public DateTime UtcDate { get; private set; }

        public int Version { get; private set; }

        
        public Dictionary<string, string> Metadata { get; private set; }

        object IConsistentHashable.ConsistentHashKey
        {
            get { return AggregateId; }
        }
    }
}