using MediatR;

namespace Edgias.MurimiOS.Domain.SharedKernel;

public abstract class BaseDomainEvent : INotification
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
