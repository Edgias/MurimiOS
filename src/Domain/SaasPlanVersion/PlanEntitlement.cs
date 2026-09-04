using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class PlanEntitlement : TenantEntity
{
    private PlanEntitlement() { }

    public PlanEntitlement(Guid id, Guid planVersionId, string key, EntitlementKind kind, decimal? includedAmount,
        string unit, EntitlementEnforcement enforcement) : base(id)
    {
        if (planVersionId == Guid.Empty || !Enum.IsDefined(kind) || !Enum.IsDefined(enforcement)
            || kind == EntitlementKind.Feature && includedAmount is not null
            || kind == EntitlementKind.Limit && includedAmount is null or < 0)
            throw new ArgumentException("Entitlement type, amount, or enforcement is invalid.");
        PlanVersionId = planVersionId;
        Key = TenantAccount.Code(key, 120);
        Kind = kind;
        IncludedAmount = includedAmount;
        Unit = TenantAccount.Text(unit, 40).ToLowerInvariant();
        Enforcement = enforcement;
    }

    public Guid PlanVersionId { get; private set; }
    public string Key { get; private set; } = "";
    public EntitlementKind Kind { get; private set; }
    public decimal? IncludedAmount { get; private set; }
    public string Unit { get; private set; } = "";
    public EntitlementEnforcement Enforcement { get; private set; }
}
