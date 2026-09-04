using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using BillingInterval = global::EdgIQ.MurimiOS.Domain.TenantSubscription.BillingInterval;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class SaasPlanVersion : TenantEntity
{
    private SaasPlanVersion() { }

    public SaasPlanVersion(Guid id, Guid seriesId, string code, string name, int planVersion,
        BillingInterval billingInterval, decimal basePrice, string currency, DateTimeOffset effectiveFrom,
        DateTimeOffset? effectiveTo, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (seriesId == Guid.Empty || planVersion < 1 || basePrice < 0 || effectiveTo <= effectiveFrom || !Enum.IsDefined(billingInterval))
            throw new ArgumentException("Plan lineage, price, interval, or effective dates are invalid.");
        SeriesId = seriesId;
        Code = TenantAccount.Code(code, 80);
        Name = TenantAccount.Text(name, 200);
        PlanVersion = planVersion;
        BillingInterval = billingInterval;
        BasePrice = basePrice;
        Currency = TenantAccount.Currency(currency);
        EffectiveFrom = effectiveFrom.ToUniversalTime();
        EffectiveTo = effectiveTo?.ToUniversalTime();
        CreatedAt = createdAt.ToUniversalTime();
        CreatedBy = TenantAccount.Text(createdBy, 240);
        Status = SaasPlanStatus.Draft;
    }

    public Guid SeriesId { get; private set; }
    public string Code { get; private set; } = "";
    public string Name { get; private set; } = "";
    public int PlanVersion { get; private set; }
    public BillingInterval BillingInterval { get; private set; }
    public decimal BasePrice { get; private set; }
    public string Currency { get; private set; } = "";
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveTo { get; private set; }
    public SaasPlanStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = "";
    public DateTimeOffset? PublishedAt { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }

    public void Publish(DateTimeOffset at)
    {
        if (Status != SaasPlanStatus.Draft) throw new InvalidOperationException("Only a draft plan can be published.");
        Status = SaasPlanStatus.Published;
        PublishedAt = at.ToUniversalTime();
    }

    public void Retire(DateTimeOffset at)
    {
        if (Status != SaasPlanStatus.Published) throw new InvalidOperationException("Only a published plan can be retired.");
        Status = SaasPlanStatus.Retired;
        RetiredAt = at.ToUniversalTime();
    }
}
