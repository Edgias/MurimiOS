using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.TenantMembership;

public enum TenantRole
{
    Owner = 1,
    Administrator = 2,
    FarmManager = 3,
    Agronomist = 4,
    LivestockManager = 5,
    Operator = 6,
    Auditor = 7,
    Buyer = 8,
    Viewer = 9
}

public enum MembershipStatus
{
    Invited = 1,
    Active = 2,
    Suspended = 3,
    Revoked = 4
}

public sealed class TenantMembership : TenantEntity
{
    private TenantMembership() { }

    public TenantMembership(
        Guid id,
        string externalSubject,
        string displayName,
        string? email,
        TenantRole role,
        MembershipStatus status,
        Guid? scopeOrganisationNodeId = null) : base(id)
    {
        if (!Enum.IsDefined(role)) throw new ArgumentOutOfRangeException(nameof(role));
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        if (scopeOrganisationNodeId == Guid.Empty) throw new ArgumentException("A scope id must be non-empty when supplied.", nameof(scopeOrganisationNodeId));

        ExternalSubject = RequireText(externalSubject, nameof(externalSubject), 240);
        DisplayName = RequireText(displayName, nameof(displayName), 200);
        Email = OptionalText(email, 320)?.ToLowerInvariant();
        Role = role;
        Status = status;
        ScopeOrganisationNodeId = scopeOrganisationNodeId;
    }

    public string ExternalSubject { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public TenantRole Role { get; private set; }
    public MembershipStatus Status { get; private set; }
    public Guid? ScopeOrganisationNodeId { get; private set; }

    public void UpdateAccess(TenantRole role, MembershipStatus status, Guid? scopeOrganisationNodeId)
    {
        if (!Enum.IsDefined(role)) throw new ArgumentOutOfRangeException(nameof(role));
        if (!Enum.IsDefined(status)) throw new ArgumentOutOfRangeException(nameof(status));
        if (scopeOrganisationNodeId == Guid.Empty) throw new ArgumentException("A scope id must be non-empty when supplied.", nameof(scopeOrganisationNodeId));
        Role = role;
        Status = status;
        ScopeOrganisationNodeId = scopeOrganisationNodeId;
    }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return result;
    }

    private static string? OptionalText(string? value, int maximumLength)
    {
        string? result = value?.Trim();
        if (result?.Length > maximumLength) throw new ArgumentException($"Value cannot exceed {maximumLength} characters.", nameof(value));
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
