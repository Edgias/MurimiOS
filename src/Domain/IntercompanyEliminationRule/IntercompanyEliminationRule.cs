using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;

public sealed class IntercompanyEliminationRule : TenantEntity
{
    private IntercompanyEliminationRule() { }

    public IntercompanyEliminationRule(Guid id, Guid consolidationOrganisationNodeId, Guid sellerCompanyId,
        Guid buyerCompanyId, string ruleKey, int ruleVersion, string name, string currency,
        decimal balancingTolerance, DateOnly effectiveFrom, DateOnly? effectiveTo, string createdBy) : base(id)
    {
        if (consolidationOrganisationNodeId == Guid.Empty || sellerCompanyId == Guid.Empty || buyerCompanyId == Guid.Empty
            || sellerCompanyId == buyerCompanyId || ruleVersion <= 0 || balancingTolerance < 0 || effectiveTo < effectiveFrom)
            throw new ArgumentException("The consolidation scope, distinct companies, version, tolerance, and effective dates must be valid.");
        ConsolidationOrganisationNodeId = consolidationOrganisationNodeId;
        SellerCompanyId = sellerCompanyId; BuyerCompanyId = buyerCompanyId;
        RuleKey = LegalCompany.Required(ruleKey, 120).ToLowerInvariant(); RuleVersion = ruleVersion;
        Name = LegalCompany.Required(name, 240); Currency = new CurrencyCode(currency).Value;
        BalancingTolerance = decimal.Round(balancingTolerance, 2, MidpointRounding.AwayFromZero);
        EffectiveFrom = effectiveFrom; EffectiveTo = effectiveTo; CreatedBy = LegalCompany.Required(createdBy, 240);
        Status = EliminationRuleStatus.Draft;
    }

    public Guid ConsolidationOrganisationNodeId { get; private set; }
    public Guid SellerCompanyId { get; private set; }
    public Guid BuyerCompanyId { get; private set; }
    public string RuleKey { get; private set; } = string.Empty;
    public int RuleVersion { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public decimal BalancingTolerance { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public EliminationRuleStatus Status { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalReference { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public string? RetiredBy { get; private set; }

    public void Approve(DateTimeOffset at, string actor, string reference)
    {
        if (Status != EliminationRuleStatus.Draft) throw new InvalidOperationException("Only a draft elimination rule can be approved.");
        Status = EliminationRuleStatus.Active; ApprovedAt = at.ToUniversalTime();
        ApprovedBy = LegalCompany.Required(actor, 240); ApprovalReference = LegalCompany.Required(reference, 240);
    }

    public void Retire(DateTimeOffset at, string actor)
    {
        if (Status != EliminationRuleStatus.Active) throw new InvalidOperationException("Only an active elimination rule can be retired.");
        Status = EliminationRuleStatus.Retired; RetiredAt = at.ToUniversalTime(); RetiredBy = LegalCompany.Required(actor, 240);
    }
}
