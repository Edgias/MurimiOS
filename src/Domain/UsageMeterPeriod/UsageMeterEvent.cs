using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class UsageMeterEvent : TenantEntity
{
    private UsageMeterEvent() { }

    public UsageMeterEvent(Guid id, string idempotencyKey, string metricCode, decimal quantity, string unit,
        DateTimeOffset occurredAt, string source, string dimensionsJson) : base(id)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        IdempotencyKey = TenantAccount.Text(idempotencyKey, 160);
        MetricCode = TenantAccount.Code(metricCode, 120);
        Quantity = quantity;
        Unit = TenantAccount.Text(unit, 40).ToLowerInvariant();
        OccurredAt = occurredAt.ToUniversalTime();
        Source = TenantAccount.Text(source, 120);
        DimensionsJson = TenantAccount.Text(dimensionsJson, 20_000);
    }

    public string IdempotencyKey { get; private set; } = "";
    public string MetricCode { get; private set; } = "";
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = "";
    public DateTimeOffset OccurredAt { get; private set; }
    public string Source { get; private set; } = "";
    public string DimensionsJson { get; private set; } = "{}";
}
