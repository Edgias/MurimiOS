using Murimi.ApplicationCore.Events;

namespace Murimi.ApplicationCore.Interfaces
{
    public interface IDomainEventDispatcher
    {
        void Dispatch(BaseDomainEvent domainEvent);
    }
}
