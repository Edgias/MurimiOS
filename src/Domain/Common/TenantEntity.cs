namespace EdgIQ.MurimiOS.Domain.Common;

public abstract class TenantEntity
{
    protected TenantEntity()
    {
    }

    protected TenantEntity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("An entity id is required.", nameof(id));
        }

        Id = id;
    }

    public Guid Id { get; protected set; }

    public Guid TenantId { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; private set; }

    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public uint Version { get; private set; }

    public void AttachToTenant(Guid tenantId, DateTimeOffset now)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("A tenant id is required.", nameof(tenantId));
        }

        if (TenantId != Guid.Empty && TenantId != tenantId)
        {
            throw new InvalidOperationException("An entity cannot be moved between tenants.");
        }

        TenantId = tenantId;
        CreatedAtUtc = CreatedAtUtc == default ? now : CreatedAtUtc;
        UpdatedAtUtc = now;
        Version = Version == 0 ? 1u : Version;
    }

    public void MarkUpdated(DateTimeOffset now)
    {
        UpdatedAtUtc = now;
        Version++;
    }
}
