using System;

namespace Akka.EventStore.Cqrs.Core
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message)
        {
        }
    }
}