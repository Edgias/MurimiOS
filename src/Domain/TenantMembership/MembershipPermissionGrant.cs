using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.TenantMembership;

public sealed class MembershipPermissionGrant : TenantEntity
{
    private MembershipPermissionGrant() { }

    public MembershipPermissionGrant(Guid id, Guid membershipId, string permission) : base(id)
    {
        if (membershipId == Guid.Empty) throw new ArgumentException("A membership is required.", nameof(membershipId));
        string normalized = permission?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!PlatformPermissions.All.Contains(normalized)) throw new ArgumentException("Unknown platform permission.", nameof(permission));
        MembershipId = membershipId;
        Permission = normalized;
    }

    public Guid MembershipId { get; private set; }
    public string Permission { get; private set; } = string.Empty;
}
