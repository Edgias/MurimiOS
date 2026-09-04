using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Application.Common.Abstractions;

namespace EdgIQ.MurimiOS.API.Infrastructure.Access;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using IRequestActorContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IRequestActorContext;

public sealed class RequestActorContext : IRequestActorContext
{
    private HashSet<string> permissions = new(StringComparer.Ordinal);

    public bool IsResolved { get; private set; }
    public string Subject { get; private set; } = string.Empty;
    public Guid? MembershipId { get; private set; }
    public TenantRole Role { get; private set; }
    public Guid? ScopeOrganisationNodeId { get; private set; }
    public IReadOnlySet<string> Permissions => permissions;

    public bool HasPermission(string permission) => permissions.Contains(permission);

    public void Resolve(
        string subject,
        Guid? membershipId,
        TenantRole role,
        Guid? scopeOrganisationNodeId,
        IEnumerable<string> effectivePermissions)
    {
        if (IsResolved) throw new InvalidOperationException("The request actor is immutable once resolved.");
        Subject = string.IsNullOrWhiteSpace(subject) ? throw new ArgumentException("An external subject is required.", nameof(subject)) : subject;
        MembershipId = membershipId;
        Role = role;
        ScopeOrganisationNodeId = scopeOrganisationNodeId;
        permissions = new HashSet<string>(effectivePermissions, StringComparer.Ordinal);
        IsResolved = true;
    }
}
