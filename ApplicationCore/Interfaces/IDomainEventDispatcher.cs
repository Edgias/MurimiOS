using Edgias.Agrik.ApplicationCore.Events;

namespace Edgias.Agrik.ApplicationCore.Interfaces
{
    public interface IDomainEventDispatcher
    {
        void Dispatch(BaseDomainEvent domainEvent);
    }
}
