using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class SupplierSelfServiceToken : TenantEntity
{
    private SupplierSelfServiceToken() { }

    public SupplierSelfServiceToken(Guid id, Guid hostedSupplierId, SupplierSelfServiceAction action,
        Guid claimedTenantId, string tokenHash, DateTimeOffset expiresAt, DateTimeOffset issuedAt,
        string issuedBy) : base(id)
    {
        if (hostedSupplierId == Guid.Empty || claimedTenantId == Guid.Empty || !Enum.IsDefined(action)
            || expiresAt <= issuedAt) throw new ArgumentException("A supplier, target tenant, action and future expiry are required.");
        HostedSupplierId = hostedSupplierId;
        Action = action;
        ClaimedTenantId = claimedTenantId;
        TokenHash = Hash(tokenHash);
        ExpiresAt = expiresAt.ToUniversalTime();
        IssuedAt = issuedAt.ToUniversalTime();
        IssuedBy = SupplierInvitation.Text(issuedBy, 240);
        Status = SupplierSelfServiceTokenStatus.Pending;
    }

    public Guid HostedSupplierId { get; private set; }
    public SupplierSelfServiceAction Action { get; private set; }
    public Guid ClaimedTenantId { get; private set; }
    public string TokenHash { get; private set; } = "";
    public SupplierSelfServiceTokenStatus Status { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public string IssuedBy { get; private set; } = "";
    public DateTimeOffset? ConsumedAt { get; private set; }
    public string? ConsumerNetworkHash { get; private set; }

    public void Consume(DateTimeOffset at, string? consumerNetworkHash)
    {
        EnsurePending(at);
        Status = SupplierSelfServiceTokenStatus.Consumed;
        ConsumedAt = at.ToUniversalTime();
        ConsumerNetworkHash = string.IsNullOrWhiteSpace(consumerNetworkHash) ? null : Hash(consumerNetworkHash);
    }

    public void Revoke(DateTimeOffset at)
    {
        if (Status != SupplierSelfServiceTokenStatus.Pending) throw new InvalidOperationException("Only a pending supplier action can be revoked.");
        Status = SupplierSelfServiceTokenStatus.Revoked;
        ConsumedAt = at.ToUniversalTime();
    }

    public void Expire(DateTimeOffset at)
    {
        if (Status != SupplierSelfServiceTokenStatus.Pending || at < ExpiresAt) throw new InvalidOperationException("The supplier action is not eligible to expire.");
        Status = SupplierSelfServiceTokenStatus.Expired;
        ConsumedAt = at.ToUniversalTime();
    }

    private void EnsurePending(DateTimeOffset at)
    {
        if (Status != SupplierSelfServiceTokenStatus.Pending) throw new InvalidOperationException("The supplier action token has already been used or revoked.");
        if (at >= ExpiresAt) throw new InvalidOperationException("The supplier action token has expired.");
    }

    private static string Hash(string? value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? "";
        if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 token hash is required.");
        return result;
    }
}
