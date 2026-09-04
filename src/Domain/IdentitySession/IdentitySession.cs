using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.IdentitySession;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed class IdentitySession : TenantEntity
{
    private IdentitySession() { }
    public IdentitySession(Guid id, string sessionKeyHash, string subject, Guid? deviceId, DateTimeOffset authenticatedAt,
        DateTimeOffset expiresAt, bool mfaSatisfied, string authenticationMethod, string? remoteAddressHash,
        string? userAgentHash) : base(id)
    {
        if (deviceId == Guid.Empty || expiresAt <= authenticatedAt) throw new ArgumentException("Session device or validity is invalid.");
        SessionKeyHash = ApiRateLimitPolicy.Hash(sessionKeyHash); Subject = ApiRateLimitPolicy.Text(subject, 240);
        DeviceId = deviceId; AuthenticatedAt = authenticatedAt.ToUniversalTime(); ExpiresAt = expiresAt.ToUniversalTime();
        LastSeenAt = AuthenticatedAt; MfaSatisfied = mfaSatisfied; AuthenticationMethod = ApiRateLimitPolicy.Text(authenticationMethod, 80);
        RemoteAddressHash = OptionalHash(remoteAddressHash); UserAgentHash = OptionalHash(userAgentHash); Status = IdentitySessionStatus.Active;
    }
    public string SessionKeyHash { get; private set; } = "";
    public string Subject { get; private set; } = "";
    public Guid? DeviceId { get; private set; }
    public IdentitySessionStatus Status { get; private set; }
    public DateTimeOffset AuthenticatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }
    public bool MfaSatisfied { get; private set; }
    public string AuthenticationMethod { get; private set; } = "";
    public string? RemoteAddressHash { get; private set; }
    public string? UserAgentHash { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public string? RevocationReason { get; private set; }
    public void Seen(DateTimeOffset at) { if (Status != IdentitySessionStatus.Active || at >= ExpiresAt) throw new InvalidOperationException("Session is not active."); LastSeenAt = at.ToUniversalTime(); }
    public void Revoke(DateTimeOffset at, string actor, string reason) { if (Status != IdentitySessionStatus.Active) throw new InvalidOperationException("Session is not active."); Status = IdentitySessionStatus.Revoked; RevokedAt = at.ToUniversalTime(); RevokedBy = ApiRateLimitPolicy.Text(actor, 240); RevocationReason = ApiRateLimitPolicy.Text(reason, 1_000); }
    public void Expire(DateTimeOffset at) { if (Status != IdentitySessionStatus.Active || at < ExpiresAt) throw new InvalidOperationException("Session is not eligible to expire."); Status = IdentitySessionStatus.Expired; }
    private static string? OptionalHash(string? value) => string.IsNullOrWhiteSpace(value) ? null : ApiRateLimitPolicy.Hash(value);
}
