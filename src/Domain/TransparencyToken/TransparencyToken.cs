using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.TransparencyToken;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class TransparencyToken : TenantEntity
{
    private TransparencyToken() { }
    public TransparencyToken(Guid id, Guid policyId, TraceObjectType objectType, Guid objectId, string tokenPrefix, string tokenHash, DateTimeOffset issuedAt, DateTimeOffset? expiresAt, string issuedBy) : base(id)
    { if (policyId == Guid.Empty || objectId == Guid.Empty || objectType != TraceObjectType.HarvestLot || expiresAt <= issuedAt) throw new ArgumentException("Only a valid harvest-lot transparency token can be issued."); PolicyId = policyId; ObjectType = objectType; ObjectId = objectId; TokenPrefix = PublicApiClient.Text(tokenPrefix, 24); TokenHash = PublicApiClient.Hash(tokenHash); IssuedAt = issuedAt.ToUniversalTime(); ExpiresAt = expiresAt?.ToUniversalTime(); IssuedBy = PublicApiClient.Text(issuedBy, 240); Status = TransparencyTokenStatus.Active; }
    public Guid PolicyId { get; private set; } public TraceObjectType ObjectType { get; private set; } public Guid ObjectId { get; private set; } public string TokenPrefix { get; private set; } = ""; public string TokenHash { get; private set; } = ""; public DateTimeOffset IssuedAt { get; private set; } public DateTimeOffset? ExpiresAt { get; private set; } public string IssuedBy { get; private set; } = ""; public TransparencyTokenStatus Status { get; private set; } public DateTimeOffset? RevokedAt { get; private set; }
    public void Revoke(DateTimeOffset at) { if (Status != TransparencyTokenStatus.Active) throw new InvalidOperationException("Token is already revoked."); Status = TransparencyTokenStatus.Revoked; RevokedAt = at.ToUniversalTime(); }
}
