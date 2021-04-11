using System;
using System.Collections.Generic;

namespace Murimi.ApplicationCore.SharedKernel
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        public bool IsActive { get; private set; } = true;

        public bool IsDeleted { get; private set; }

        public List<BaseDomainEvent> Events { get; private set; } = new List<BaseDomainEvent>();

        public void AddDomainEvent(BaseDomainEvent domainEvent)
        {
           Events.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Remove(domainEvent);
        }

        public void ChangeStatus()
        {
            IsActive = !IsActive;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

    }
}
