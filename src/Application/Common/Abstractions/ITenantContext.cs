namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public interface ITenantContext
{
    Guid TenantId { get; }

    bool IsResolved { get; }
}
