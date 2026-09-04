using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.PublicApiClient;

public sealed class PublicApiCredential : TenantEntity
{
    private PublicApiCredential() { }
    public PublicApiCredential(Guid id, Guid clientId, int credentialVersion, string keyPrefix, string keyHash,
        DateTimeOffset validFrom, DateTimeOffset? expiresAt, DateTimeOffset createdAt, string createdBy, Guid? previousCredentialId = null) : base(id)
    {
        if (clientId == Guid.Empty || previousCredentialId == Guid.Empty || credentialVersion <= 0 || expiresAt <= validFrom) throw new ArgumentException("Credential lineage, version, or validity is invalid.");
        ClientId = clientId; CredentialVersion = credentialVersion; KeyPrefix = PublicApiClient.Text(keyPrefix, 24);
        KeyHash = PublicApiClient.Hash(keyHash); ValidFrom = validFrom.ToUniversalTime(); ExpiresAt = expiresAt?.ToUniversalTime();
        IssuedAt = createdAt.ToUniversalTime(); IssuedBy = PublicApiClient.Text(createdBy, 240); PreviousCredentialId = previousCredentialId;
        Status = PublicApiCredentialStatus.Active;
    }
    public Guid ClientId { get; private set; }
    public int CredentialVersion { get; private set; }
    public string KeyPrefix { get; private set; } = string.Empty;
    public string KeyHash { get; private set; } = string.Empty;
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public string IssuedBy { get; private set; } = string.Empty;
    public Guid? PreviousCredentialId { get; private set; }
    public PublicApiCredentialStatus Status { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }
    public Guid? SupersededByCredentialId { get; private set; }
    public void Supersede(Guid replacementId, DateTimeOffset at) { if (Status != PublicApiCredentialStatus.Active || replacementId == Guid.Empty || replacementId == Id) throw new InvalidOperationException("Only an active credential can be superseded."); Status = PublicApiCredentialStatus.Superseded; SupersededByCredentialId = replacementId; EndedAt = at.ToUniversalTime(); }
    public void Revoke(DateTimeOffset at) { if (Status != PublicApiCredentialStatus.Active) throw new InvalidOperationException("Only an active credential can be revoked."); Status = PublicApiCredentialStatus.Revoked; EndedAt = at.ToUniversalTime(); }
}
