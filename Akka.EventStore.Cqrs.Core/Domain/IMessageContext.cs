using System;
using System.Collections;
using System.Collections.Generic;

namespace Akka.EventStore.Cqrs.Core
{
    public interface IMessageContext
    {
        Dictionary<string, string> Metadata { get; }
    }
}