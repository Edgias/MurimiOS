

using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using IMutableTenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IMutableTenantContext;

public sealed class HttpTenantContext : IMutableTenantContext
{
    private Guid? _tenantId;

    public Guid TenantId => _tenantId ?? Guid.Empty;

    public bool IsResolved => _tenantId.HasValue;

    public void Resolve(Guid tenantId)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("A tenant id is required.", nameof(tenantId));
        }

        if (_tenantId.HasValue && _tenantId.Value != tenantId)
        {
            throw new InvalidOperationException("The tenant context is immutable for a request.");
        }

        _tenantId = tenantId;
    }
}
