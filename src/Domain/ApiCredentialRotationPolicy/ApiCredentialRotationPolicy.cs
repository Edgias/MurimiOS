using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed class ApiCredentialRotationPolicy : TenantEntity
{
    private ApiCredentialRotationPolicy() { }
    public ApiCredentialRotationPolicy(Guid id, Guid? publicApiClientId, int maximumAgeDays, int warningDays,
        bool requireExpiry, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (publicApiClientId == Guid.Empty || maximumAgeDays is < 1 or > 730 || warningDays < 0 || warningDays >= maximumAgeDays)
            throw new ArgumentException("Credential scope, maximum age, or warning period is invalid.");
        PublicApiClientId = publicApiClientId; MaximumAgeDays = maximumAgeDays; WarningDays = warningDays;
        RequireExpiry = requireExpiry; CreatedAt = createdAt.ToUniversalTime(); CreatedBy = ApiRateLimitPolicy.Text(createdBy, 240); IsActive = true;
    }
    public Guid? PublicApiClientId { get; private set; }
    public int MaximumAgeDays { get; private set; }
    public int WarningDays { get; private set; }
    public bool RequireExpiry { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = "";
    public bool IsActive { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void Retire(DateTimeOffset at) { if (!IsActive) throw new InvalidOperationException("Policy is already retired."); IsActive = false; RetiredAt = at.ToUniversalTime(); }
}
