using Murimi.ApplicationCore.Events;
using System;
using System.Threading.Tasks;

namespace Murimi.ApplicationCore.Interfaces
{
    public interface IDomainEventHandler
    {
        Task HandleAsync<T>(T domainEvent) where T : BaseDomainEvent;

        bool AppliesTo(Type handler);
    }
}
