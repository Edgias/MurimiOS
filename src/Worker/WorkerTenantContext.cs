using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.Worker;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class WorkerTenantContext : ITenantContext
{
    private Guid? tenantId;
    public Guid TenantId => tenantId ?? Guid.Empty;
    public bool IsResolved => tenantId.HasValue;
    public void Resolve(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("A tenant id is required.", nameof(value));
        if (tenantId is not null && tenantId != value) throw new InvalidOperationException("The worker scope is already assigned to another tenant.");
        tenantId = value;
    }
}
