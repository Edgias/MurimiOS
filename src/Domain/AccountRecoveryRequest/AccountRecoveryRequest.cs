using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed class AccountRecoveryRequest : TenantEntity
{
    private AccountRecoveryRequest() { }
    public AccountRecoveryRequest(Guid id, Guid membershipId, string subject, string tokenPrefix, string tokenHash,
        DateTimeOffset requestedAt, DateTimeOffset expiresAt, int maximumAttempts, string requestedBy) : base(id)
    {
        if (membershipId == Guid.Empty || expiresAt <= requestedAt || maximumAttempts is < 1 or > 20)
            throw new ArgumentException("Recovery membership, expiry, or attempt policy is invalid.");
        MembershipId = membershipId; Subject = ApiRateLimitPolicy.Text(subject, 240); TokenPrefix = ApiRateLimitPolicy.Text(tokenPrefix, 24);
        TokenHash = ApiRateLimitPolicy.Hash(tokenHash); RequestedAt = requestedAt.ToUniversalTime();
        ExpiresAt = expiresAt.ToUniversalTime(); MaximumAttempts = maximumAttempts;
        RequestedBy = ApiRateLimitPolicy.Text(requestedBy, 240); Status = AccountRecoveryStatus.Pending;
    }
    public Guid MembershipId { get; private set; }
    public string Subject { get; private set; } = "";
    public string TokenPrefix { get; private set; } = "";
    public string TokenHash { get; private set; } = "";
    public AccountRecoveryStatus Status { get; private set; }
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public int MaximumAttempts { get; private set; }
    public int FailedAttempts { get; private set; }
    public string RequestedBy { get; private set; } = "";
    public DateTimeOffset? VerifiedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }
    public void RecordFailure(DateTimeOffset at) { EnsurePending(at); FailedAttempts++; if (FailedAttempts >= MaximumAttempts) { Status = AccountRecoveryStatus.Locked; ClosedAt = at.ToUniversalTime(); } }
    public void Verify(DateTimeOffset at) { EnsurePending(at); Status = AccountRecoveryStatus.Verified; VerifiedAt = at.ToUniversalTime(); ClosedAt = VerifiedAt; }
    public void Cancel(DateTimeOffset at, string actor) { if (Status != AccountRecoveryStatus.Pending) throw new InvalidOperationException("Only pending recovery can be cancelled."); Status = AccountRecoveryStatus.Cancelled; ClosedAt = at.ToUniversalTime(); ClosedBy = ApiRateLimitPolicy.Text(actor, 240); }
    private void EnsurePending(DateTimeOffset at) { if (Status != AccountRecoveryStatus.Pending) throw new InvalidOperationException("Recovery request is not pending."); if (at >= ExpiresAt) { Status = AccountRecoveryStatus.Expired; ClosedAt = at.ToUniversalTime(); throw new InvalidOperationException("Recovery request has expired."); } }
}
