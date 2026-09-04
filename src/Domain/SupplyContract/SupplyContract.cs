using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplyContract;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class SupplyContract : TenantEntity
{
    private SupplyContract() { }
    public SupplyContract(Guid id, string contractNumber, Guid? relationshipId, Guid? hostedSupplierId, string commodity,
        DateOnly validFrom, DateOnly validTo, decimal contractedQuantity, string quantityUnit, decimal? unitPrice,
        string? currency, string? termsJson) : base(id)
    {
        if (relationshipId.HasValue == hostedSupplierId.HasValue) throw new ArgumentException("Choose either a tenant relationship or hosted supplier.");
        if (relationshipId == Guid.Empty || hostedSupplierId == Guid.Empty || validTo < validFrom || contractedQuantity <= 0 || unitPrice < 0) throw new ArgumentOutOfRangeException(nameof(contractedQuantity));
        if (unitPrice.HasValue != !string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Price and currency must be supplied together.");
        ContractNumber = SupplierInvitation.Text(contractNumber, 80); RelationshipId = relationshipId; HostedSupplierId = hostedSupplierId;
        Commodity = SupplierInvitation.Text(commodity, 120); ValidFrom = validFrom; ValidTo = validTo; ContractedQuantity = contractedQuantity;
        QuantityUnit = SupplierInvitation.Text(quantityUnit, 30); UnitPrice = unitPrice; Currency = currency?.Trim().ToUpperInvariant();
        TermsJson = SupplierInvitation.Optional(termsJson, 20_000); Status = SupplyContractStatus.Draft;
    }
    public string ContractNumber { get; private set; } = string.Empty;
    public Guid? RelationshipId { get; private set; }
    public Guid? HostedSupplierId { get; private set; }
    public string Commodity { get; private set; } = string.Empty;
    public DateOnly ValidFrom { get; private set; }
    public DateOnly ValidTo { get; private set; }
    public decimal ContractedQuantity { get; private set; }
    public string QuantityUnit { get; private set; } = string.Empty;
    public decimal? UnitPrice { get; private set; }
    public string? Currency { get; private set; }
    public string? TermsJson { get; private set; }
    public SupplyContractStatus Status { get; private set; }
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? TerminationReason { get; private set; }
    public void Activate(DateTimeOffset at) { if (Status != SupplyContractStatus.Draft) throw new InvalidOperationException("Only a draft contract can be activated."); Status = SupplyContractStatus.Active; ActivatedAt = at.ToUniversalTime(); }
    public void Complete(DateTimeOffset at) { if (Status != SupplyContractStatus.Active) throw new InvalidOperationException("Only an active contract can complete."); Status = SupplyContractStatus.Completed; ClosedAt = at.ToUniversalTime(); }
    public void Terminate(DateTimeOffset at, string reason) { if (Status is SupplyContractStatus.Completed or SupplyContractStatus.Terminated) throw new InvalidOperationException("The contract is already closed."); Status = SupplyContractStatus.Terminated; ClosedAt = at.ToUniversalTime(); TerminationReason = SupplierInvitation.Text(reason, 2_000); }
}
