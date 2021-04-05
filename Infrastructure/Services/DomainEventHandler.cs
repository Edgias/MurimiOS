using Murimi.ApplicationCore.Events;
using Murimi.ApplicationCore.Interfaces;
using System;
using System.Threading.Tasks;

namespace Murimi.Infrastructure.Services
{
    public abstract class DomainEventHandler<T> : IDomainEventHandler where T : BaseDomainEvent
    {
        public bool AppliesTo(Type handler)
        {
            return typeof(T).Equals(handler);
        }

        public async Task HandleAsync<T1>(T1 domainEvent) where T1 : BaseDomainEvent
        {
            await HandleAsync((T)(object)domainEvent);
        }

        protected abstract Task HandleAsync(T domainEvent);
    }
}
