namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public interface IMutableTenantContext : ITenantContext
{
    void Resolve(Guid tenantId);
}
