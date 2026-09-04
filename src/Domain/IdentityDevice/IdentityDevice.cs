using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.IdentityDevice;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;

public sealed class IdentityDevice : TenantEntity
{
    private IdentityDevice() { }
    public IdentityDevice(Guid id, string deviceKeyHash, string subject, string name, string platform,
        DateTimeOffset firstSeenAt) : base(id)
    {
        DeviceKeyHash = ApiRateLimitPolicy.Hash(deviceKeyHash); Subject = ApiRateLimitPolicy.Text(subject, 240);
        Name = ApiRateLimitPolicy.Text(name, 200); Platform = ApiRateLimitPolicy.Text(platform, 80);
        FirstSeenAt = firstSeenAt.ToUniversalTime(); LastSeenAt = FirstSeenAt; Status = IdentityDeviceStatus.Active;
    }
    public string DeviceKeyHash { get; private set; } = "";
    public string Subject { get; private set; } = "";
    public string Name { get; private set; } = "";
    public string Platform { get; private set; } = "";
    public IdentityDeviceStatus Status { get; private set; }
    public DateTimeOffset FirstSeenAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public string? RevocationReason { get; private set; }
    public void Seen(DateTimeOffset at) { if (Status != IdentityDeviceStatus.Active) throw new InvalidOperationException("Revoked device cannot be used."); LastSeenAt = at.ToUniversalTime(); }
    public void Revoke(DateTimeOffset at, string actor, string reason) { if (Status == IdentityDeviceStatus.Revoked) throw new InvalidOperationException("Device is already revoked."); Status = IdentityDeviceStatus.Revoked; RevokedAt = at.ToUniversalTime(); RevokedBy = ApiRateLimitPolicy.Text(actor, 240); RevocationReason = ApiRateLimitPolicy.Text(reason, 1_000); }
}
