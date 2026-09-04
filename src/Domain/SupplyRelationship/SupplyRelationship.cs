

namespace EdgIQ.MurimiOS.Domain.SupplyRelationship;

public sealed class SupplyRelationship
{
    private SupplyRelationship() { }
    public SupplyRelationship(Guid id, Guid producerTenantId, Guid buyerTenantId, Guid requestedByTenantId,
        string relationshipName, DateTimeOffset requestedAt)
    {
        if (id == Guid.Empty || producerTenantId == Guid.Empty || buyerTenantId == Guid.Empty || requestedByTenantId == Guid.Empty)
            throw new ArgumentException("Relationship and tenant identifiers are required.");
        if (producerTenantId == buyerTenantId) throw new ArgumentException("A supply relationship requires two distinct tenants.");
        if (requestedByTenantId != producerTenantId && requestedByTenantId != buyerTenantId)
            throw new ArgumentException("The requester must be a relationship party.");
        Id = id; ProducerTenantId = producerTenantId; BuyerTenantId = buyerTenantId;
        RequestedByTenantId = requestedByTenantId; RelationshipName = Text(relationshipName, nameof(relationshipName), 200);
        RequestedAt = requestedAt; CreatedAtUtc = requestedAt; UpdatedAtUtc = requestedAt; Status = SupplyRelationshipStatus.Pending; Version = 1;
    }
    public Guid Id { get; private set; }
    public Guid ProducerTenantId { get; private set; }
    public Guid BuyerTenantId { get; private set; }
    public Guid RequestedByTenantId { get; private set; }
    public string RelationshipName { get; private set; } = string.Empty;
    public SupplyRelationshipStatus Status { get; private set; }
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? RespondedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? RevokedByTenantId { get; private set; }
    public string? RevocationReason { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }
    public uint Version { get; private set; }

    public void Accept(Guid actorTenantId, DateTimeOffset at)
    {
        EnsureCounterparty(actorTenantId); if (Status != SupplyRelationshipStatus.Pending) throw new InvalidOperationException("Only a pending relationship can be accepted.");
        Status = SupplyRelationshipStatus.Active; RespondedAt = at; Touch(at);
    }
    public void Reject(Guid actorTenantId, DateTimeOffset at)
    {
        EnsureCounterparty(actorTenantId); if (Status != SupplyRelationshipStatus.Pending) throw new InvalidOperationException("Only a pending relationship can be rejected.");
        Status = SupplyRelationshipStatus.Rejected; RespondedAt = at; Touch(at);
    }
    public void Revoke(Guid actorTenantId, DateTimeOffset at, string reason)
    {
        EnsureParty(actorTenantId); if (Status != SupplyRelationshipStatus.Active) throw new InvalidOperationException("Only an active relationship can be revoked.");
        Status = SupplyRelationshipStatus.Revoked; RevokedAt = at; RevokedByTenantId = actorTenantId;
        RevocationReason = Text(reason, nameof(reason), 1_000); Touch(at);
    }
    private void EnsureCounterparty(Guid actor) { EnsureParty(actor); if (actor == RequestedByTenantId) throw new InvalidOperationException("The requesting tenant cannot accept its own relationship request."); }
    private void EnsureParty(Guid actor) { if (actor != ProducerTenantId && actor != BuyerTenantId) throw new InvalidOperationException("Tenant is not a relationship party."); }
    private void Touch(DateTimeOffset at) { UpdatedAtUtc = at; Version++; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}
