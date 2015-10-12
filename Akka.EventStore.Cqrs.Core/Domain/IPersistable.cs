using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akka.EventStore.Cqrs.Core.Domain
{
    public interface IPersistable
    {
        string PersistenceId { get; }
    }
}
