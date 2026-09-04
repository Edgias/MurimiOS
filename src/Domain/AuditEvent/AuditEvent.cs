namespace EdgIQ.MurimiOS.Domain.AuditEvent;

public sealed class AuditEvent
{
    private AuditEvent() { }

    public AuditEvent(
        Guid id,
        Guid tenantId,
        DateTimeOffset occurredAt,
        string actorSubject,
        Guid? membershipId,
        string action,
        string entityType,
        Guid entityId,
        Guid? farmId,
        Guid? organisationNodeId,
        string changes,
        string? correlationId)
    {
        if (id == Guid.Empty || tenantId == Guid.Empty || entityId == Guid.Empty)
            throw new ArgumentException("Audit, tenant, and entity identifiers are required.");
        Id = id;
        TenantId = tenantId;
        OccurredAt = occurredAt.ToUniversalTime();
        ActorSubject = RequireText(actorSubject, nameof(actorSubject), 240);
        MembershipId = membershipId;
        Action = RequireText(action, nameof(action), 40);
        EntityType = RequireText(entityType, nameof(entityType), 240);
        EntityId = entityId;
        FarmId = farmId == Guid.Empty ? null : farmId;
        OrganisationNodeId = organisationNodeId == Guid.Empty ? null : organisationNodeId;
        Changes = RequireText(changes, nameof(changes), 1_000_000);
        CorrelationId = OptionalText(correlationId, 160);
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string ActorSubject { get; private set; } = string.Empty;
    public Guid? MembershipId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public Guid? FarmId { get; private set; }
    public Guid? OrganisationNodeId { get; private set; }
    public string Changes { get; private set; } = string.Empty;
    public string? CorrelationId { get; private set; }

    private static string RequireText(string? value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return result;
    }

    private static string? OptionalText(string? value, int maximumLength)
    {
        string? result = value?.Trim();
        if (result?.Length > maximumLength) throw new ArgumentException($"Value cannot exceed {maximumLength} characters.", nameof(value));
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
