using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;

public interface IRequestActorContext : IAuditActorContext
{
    TenantRole Role { get; }
    IReadOnlySet<string> Permissions { get; }
    bool HasPermission(string permission);
}
