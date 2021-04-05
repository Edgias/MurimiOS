using Murimi.ApplicationCore.Events;
using System;
using System.Collections.Generic;

namespace Murimi.ApplicationCore.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset LastModifiedDate { get; set; } = DateTimeOffset.Now;

        public bool IsActive { get; private set; } = true;

        public bool IsDeleted { get; private set; }

        public string CreatedBy { get; set; }

        public string LastModifiedBy { get; set; }

        public List<BaseDomainEvent> Events { get; private set; } = new List<BaseDomainEvent>();

        public void AddDomainEvent(BaseDomainEvent domainEvent)
        {
           Events.Add(domainEvent);
        }

        public void RemoveDomainEvent(BaseDomainEvent domainEvent)
        {
            Events.Remove(domainEvent);
        }

        public void ChangeStatus(string userId)
        {
            LastModifiedBy = userId;
            LastModifiedDate = DateTimeOffset.Now;
            IsActive = !IsActive;
        }

        public void Delete(string userId)
        {
            IsDeleted = true;
            LastModifiedBy = userId;
            LastModifiedDate = DateTimeOffset.Now;
        }

    }
}
