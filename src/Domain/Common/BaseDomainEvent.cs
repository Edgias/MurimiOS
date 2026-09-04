using MediatR;

namespace EdgIQ.MurimiOS.Domain.Common;

public abstract class BaseDomainEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
