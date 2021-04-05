using Edgias.Agrik.ApplicationCore.Events;
using System;
using System.Threading.Tasks;

namespace Edgias.Agrik.ApplicationCore.Interfaces
{
    public interface IDomainEventHandler
    {
        Task HandleAsync<T>(T domainEvent) where T : BaseDomainEvent;

        bool AppliesTo(Type handler);
    }
}
