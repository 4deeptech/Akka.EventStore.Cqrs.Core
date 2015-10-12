namespace Akka.EventStore.Cqrs.Core
{
    public interface IEventSink
    {
        void Publish(IEvent @event);
    }
}