using System;

namespace Edgias.Agrik.ApplicationCore.Events
{
    public abstract class BaseDomainEvent
    {
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
