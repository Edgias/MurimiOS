

namespace EdgIQ.MurimiOS.Domain.DataGrant;

public sealed class CrossTenantAccessAudit
{
    private CrossTenantAccessAudit() { }
    public CrossTenantAccessAudit(Guid id, Guid producerTenantId, Guid recipientTenantId, Guid grantId,
        Guid actorTenantId, string actorSubject, string purpose, string entityCategory, Guid? resourceId,
        DateTimeOffset occurredAt, int resultCount, string outcome, string? correlationId)
    {
        if (id == Guid.Empty || producerTenantId == Guid.Empty || recipientTenantId == Guid.Empty || grantId == Guid.Empty || actorTenantId == Guid.Empty)
            throw new ArgumentException("Audit and tenant identifiers are required.");
        if (actorTenantId != producerTenantId && actorTenantId != recipientTenantId) throw new ArgumentException("Audit actor must be a grant party.");
        if (resultCount < 0) throw new ArgumentOutOfRangeException(nameof(resultCount));
        Id = id; ProducerTenantId = producerTenantId; RecipientTenantId = recipientTenantId; GrantId = grantId;
        ActorTenantId = actorTenantId; ActorSubject = Text(actorSubject, nameof(actorSubject), 240);
        Purpose = Text(purpose, nameof(purpose), 240); EntityCategory = Text(entityCategory, nameof(entityCategory), 120);
        ResourceId = resourceId; OccurredAt = occurredAt; ResultCount = resultCount; Outcome = Text(outcome, nameof(outcome), 40);
        CorrelationId = Optional(correlationId, 160);
    }
    public Guid Id { get; private set; }
    public Guid ProducerTenantId { get; private set; }
    public Guid RecipientTenantId { get; private set; }
    public Guid GrantId { get; private set; }
    public Guid ActorTenantId { get; private set; }
    public string ActorSubject { get; private set; } = string.Empty;
    public string Purpose { get; private set; } = string.Empty;
    public string EntityCategory { get; private set; } = string.Empty;
    public Guid? ResourceId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public int ResultCount { get; private set; }
    public string Outcome { get; private set; } = string.Empty;
    public string? CorrelationId { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
