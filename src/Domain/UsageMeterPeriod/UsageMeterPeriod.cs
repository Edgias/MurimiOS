using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class UsageMeterPeriod : TenantEntity
{
    private UsageMeterPeriod() { }

    public UsageMeterPeriod(Guid id, string metricCode, string unit, DateTimeOffset periodStartsAt,
        DateTimeOffset periodEndsAt, decimal initialQuantity, DateTimeOffset updatedAt) : base(id)
    {
        if (periodEndsAt <= periodStartsAt || initialQuantity <= 0) throw new ArgumentException("Usage period or quantity is invalid.");
        MetricCode = TenantAccount.Code(metricCode, 120);
        Unit = TenantAccount.Text(unit, 40).ToLowerInvariant();
        PeriodStartsAt = periodStartsAt.ToUniversalTime();
        PeriodEndsAt = periodEndsAt.ToUniversalTime();
        Quantity = initialQuantity;
        LastMeteredAt = updatedAt.ToUniversalTime();
    }

    public string MetricCode { get; private set; } = "";
    public string Unit { get; private set; } = "";
    public DateTimeOffset PeriodStartsAt { get; private set; }
    public DateTimeOffset PeriodEndsAt { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTimeOffset LastMeteredAt { get; private set; }

    public void Add(decimal quantity, DateTimeOffset at)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Quantity += quantity;
        LastMeteredAt = at.ToUniversalTime();
    }
}
