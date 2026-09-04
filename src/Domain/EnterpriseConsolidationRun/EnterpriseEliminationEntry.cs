using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;

public sealed class EnterpriseEliminationEntry : TenantEntity
{
    private EnterpriseEliminationEntry() { }

    public EnterpriseEliminationEntry(Guid id, Guid consolidationRunId, Guid ruleId, string intercompanyReference,
        decimal sellerRevenue, decimal buyerExpense, decimal eliminatedAmount, decimal difference,
        decimal tolerance, string sellerTransactionIdsJson, string buyerTransactionIdsJson) : base(id)
    {
        if (consolidationRunId == Guid.Empty || ruleId == Guid.Empty || sellerRevenue < 0 || buyerExpense < 0
            || eliminatedAmount < 0 || tolerance < 0) throw new ArgumentException("The run, rule and non-negative amounts are required.");
        ConsolidationRunId = consolidationRunId; RuleId = ruleId;
        IntercompanyReference = LegalCompany.Required(intercompanyReference, 160).ToUpperInvariant();
        SellerRevenue = sellerRevenue; BuyerExpense = buyerExpense; EliminatedAmount = eliminatedAmount;
        Difference = difference; Tolerance = tolerance;
        SellerTransactionIdsJson = LegalCompany.Required(sellerTransactionIdsJson, 16_000);
        BuyerTransactionIdsJson = LegalCompany.Required(buyerTransactionIdsJson, 16_000);
        Status = Math.Abs(difference) <= tolerance ? ConsolidationEntryStatus.Balanced : ConsolidationEntryStatus.Exception;
    }

    public Guid ConsolidationRunId { get; private set; }
    public Guid RuleId { get; private set; }
    public string IntercompanyReference { get; private set; } = string.Empty;
    public decimal SellerRevenue { get; private set; }
    public decimal BuyerExpense { get; private set; }
    public decimal EliminatedAmount { get; private set; }
    public decimal Difference { get; private set; }
    public decimal Tolerance { get; private set; }
    public ConsolidationEntryStatus Status { get; private set; }
    public string SellerTransactionIdsJson { get; private set; } = "[]";
    public string BuyerTransactionIdsJson { get; private set; } = "[]";
}
