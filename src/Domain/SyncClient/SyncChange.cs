namespace EdgIQ.MurimiOS.Domain.SyncClient;

public sealed class SyncChange
{
    private SyncChange()
    {
    }

    public SyncChange(
        Guid tenantId,
        Guid farmId,
        string entityType,
        Guid entityId,
        string operation,
        string payload,
        DateTimeOffset changedAt)
    {
        TenantId = tenantId;
        if (farmId == Guid.Empty) throw new ArgumentException("A farm scope is required.", nameof(farmId));
        FarmId = farmId;
        EntityType = entityType;
        EntityId = entityId;
        Operation = operation;
        Payload = payload;
        ChangedAt = changedAt;
    }

    public long Id { get; private set; }

    public Guid TenantId { get; private set; }

    public Guid? FarmId { get; private set; }

    public string EntityType { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    public string Operation { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTimeOffset ChangedAt { get; private set; }
}
