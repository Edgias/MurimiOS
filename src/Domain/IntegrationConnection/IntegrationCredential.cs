using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IntegrationConnection;

public sealed class IntegrationCredential : TenantEntity
{
    private IntegrationCredential() { }
    public IntegrationCredential(Guid id, Guid connectionId, int version, string secretProvider,
        string protectedReference, string fingerprint, DateTimeOffset validFrom, DateTimeOffset? expiresAt,
        DateTimeOffset createdAt, string createdBy, Guid? previousCredentialId = null) : base(id)
    {
        if (connectionId == Guid.Empty || previousCredentialId == Guid.Empty) throw new ArgumentException("Connection is required and an optional previous credential id must be non-empty.");
        if (version <= 0 || expiresAt <= validFrom) throw new ArgumentException("Credential version must be positive and expiry must follow validity.");
        ConnectionId = connectionId; CredentialVersion = version; SecretProvider = Text(secretProvider, nameof(secretProvider), 80);
        ProtectedReference = Text(protectedReference, nameof(protectedReference), 4_000);
        Fingerprint = Text(fingerprint, nameof(fingerprint), 64); ValidFrom = validFrom.ToUniversalTime();
        ExpiresAt = expiresAt?.ToUniversalTime(); CreatedAt = createdAt.ToUniversalTime(); CreatedBy = Text(createdBy, nameof(createdBy), 240);
        PreviousCredentialId = previousCredentialId; Status = IntegrationCredentialStatus.Active;
    }
    public Guid ConnectionId { get; private set; }
    public int CredentialVersion { get; private set; }
    public string SecretProvider { get; private set; } = string.Empty;
    public string ProtectedReference { get; private set; } = string.Empty;
    public string Fingerprint { get; private set; } = string.Empty;
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public Guid? PreviousCredentialId { get; private set; }
    public IntegrationCredentialStatus Status { get; private set; }
    public DateTimeOffset? SupersededAt { get; private set; }
    public Guid? SupersededByCredentialId { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public void Supersede(Guid replacementId, DateTimeOffset at)
    {
        if (Status != IntegrationCredentialStatus.Active || replacementId == Guid.Empty || replacementId == Id) throw new InvalidOperationException("Only an active credential can be superseded by a distinct replacement.");
        Status = IntegrationCredentialStatus.Superseded; SupersededAt = at.ToUniversalTime(); SupersededByCredentialId = replacementId;
    }
    public void Revoke(DateTimeOffset at, string actor)
    {
        if (Status != IntegrationCredentialStatus.Active) throw new InvalidOperationException("Only an active credential can be revoked.");
        Status = IntegrationCredentialStatus.Revoked; RevokedAt = at.ToUniversalTime(); RevokedBy = Text(actor, nameof(actor), 240);
    }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}
