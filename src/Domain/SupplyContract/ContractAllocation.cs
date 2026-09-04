using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplyContract;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class ContractAllocation : TenantEntity
{
    private ContractAllocation() { }
    public ContractAllocation(Guid id, Guid contractId, string reference, DateOnly deliveryFrom, DateOnly deliveryTo,
        decimal allocatedQuantity, string quantityUnit) : base(id)
    {
        if (contractId == Guid.Empty || deliveryTo < deliveryFrom || allocatedQuantity <= 0) throw new ArgumentOutOfRangeException(nameof(allocatedQuantity));
        ContractId = contractId; Reference = SupplierInvitation.Text(reference, 120); DeliveryFrom = deliveryFrom; DeliveryTo = deliveryTo;
        AllocatedQuantity = allocatedQuantity; QuantityUnit = SupplierInvitation.Text(quantityUnit, 30); Status = ContractAllocationStatus.Planned;
    }
    public Guid ContractId { get; private set; }
    public string Reference { get; private set; } = string.Empty;
    public DateOnly DeliveryFrom { get; private set; }
    public DateOnly DeliveryTo { get; private set; }
    public decimal AllocatedQuantity { get; private set; }
    public decimal FulfilledQuantity { get; private set; }
    public string QuantityUnit { get; private set; } = string.Empty;
    public ContractAllocationStatus Status { get; private set; }
    public void RecordFulfilment(decimal quantity) { if (Status is ContractAllocationStatus.Fulfilled or ContractAllocationStatus.Cancelled || quantity <= 0 || FulfilledQuantity + quantity > AllocatedQuantity) throw new InvalidOperationException("The fulfilment quantity is invalid."); FulfilledQuantity += quantity; Status = FulfilledQuantity == AllocatedQuantity ? ContractAllocationStatus.Fulfilled : ContractAllocationStatus.InProgress; }
    public void Cancel() { if (Status == ContractAllocationStatus.Fulfilled) throw new InvalidOperationException("A fulfilled allocation cannot be cancelled."); Status = ContractAllocationStatus.Cancelled; }
}
