using System;

namespace Akka.EventStore.Cqrs.Core
{
    public interface ICommand 
    {
        Guid AggregateId { get; }
    }
}