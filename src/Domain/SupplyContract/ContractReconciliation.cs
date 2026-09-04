using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplyContract;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class ContractReconciliation : TenantEntity
{
    private ContractReconciliation() { }

    public ContractReconciliation(Guid id, Guid contractId, int revision, string reference,
        Guid? priceRuleId, decimal fulfilledQuantity, decimal acceptedQuantity, decimal rejectedQuantity,
        string quantityUnit, decimal unitPrice, decimal grossAmount, decimal qualityAdjustmentAmount,
        decimal otherAdjustmentAmount, decimal payableAmount, string currency,
        ContractReconciliationStatus status, string? notes, DateTimeOffset reconciledAt,
        string reconciledBy) : base(id)
    {
        if (contractId == Guid.Empty || priceRuleId == Guid.Empty || revision < 1 || fulfilledQuantity < 0
            || acceptedQuantity < 0 || rejectedQuantity < 0 || acceptedQuantity + rejectedQuantity != fulfilledQuantity
            || unitPrice < 0 || grossAmount != decimal.Round(acceptedQuantity * unitPrice, 2, MidpointRounding.AwayFromZero)
            || payableAmount != decimal.Round(grossAmount + qualityAdjustmentAmount + otherAdjustmentAmount, 2, MidpointRounding.AwayFromZero)
            || payableAmount < 0 || !Enum.IsDefined(status))
            throw new ArgumentException("The contract reconciliation quantities or amounts are inconsistent.");
        ContractId = contractId;
        Revision = revision;
        Reference = SupplierInvitation.Text(reference, 120);
        PriceRuleId = priceRuleId;
        FulfilledQuantity = fulfilledQuantity;
        AcceptedQuantity = acceptedQuantity;
        RejectedQuantity = rejectedQuantity;
        QuantityUnit = SupplierInvitation.Text(quantityUnit, 30);
        UnitPrice = unitPrice;
        GrossAmount = grossAmount;
        QualityAdjustmentAmount = qualityAdjustmentAmount;
        OtherAdjustmentAmount = otherAdjustmentAmount;
        PayableAmount = payableAmount;
        Currency = ContractPriceRule.CurrencyCode(currency);
        Status = status;
        Notes = SupplierInvitation.Optional(notes, 2_000);
        ReconciledAt = reconciledAt.ToUniversalTime();
        ReconciledBy = SupplierInvitation.Text(reconciledBy, 240);
    }

    public Guid ContractId { get; private set; }
    public int Revision { get; private set; }
    public string Reference { get; private set; } = "";
    public Guid? PriceRuleId { get; private set; }
    public decimal FulfilledQuantity { get; private set; }
    public decimal AcceptedQuantity { get; private set; }
    public decimal RejectedQuantity { get; private set; }
    public string QuantityUnit { get; private set; } = "";
    public decimal UnitPrice { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal QualityAdjustmentAmount { get; private set; }
    public decimal OtherAdjustmentAmount { get; private set; }
    public decimal PayableAmount { get; private set; }
    public string Currency { get; private set; } = "";
    public ContractReconciliationStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset ReconciledAt { get; private set; }
    public string ReconciledBy { get; private set; } = "";
}
