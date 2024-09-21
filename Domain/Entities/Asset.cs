using Edgias.MurimiOS.Domain.Entities.PurchaseInvoiceAggregate;

namespace Edgias.MurimiOS.Domain.Entities;

public class Asset(string name, string serialNumber, string description, DateTime? purchaseDate,
        decimal? purchasePrice, Guid? supplierId, Guid? purchaseInvoiceId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string SerialNumber { get; private set; } = serialNumber;

    public string Description { get; private set; } = description;

    public DateTime? PurchaseDate { get; private set; } = purchaseDate;

    public decimal? PurchasePrice { get; private set; } = purchasePrice;

    public Guid? SupplierId { get; private set; } = supplierId;

    public Supplier? Supplier { get; private set; }

    public Guid? PurchaseInvoiceId { get; private set; } = purchaseInvoiceId;

    public PurchaseInvoice? PurchaseInvoice { get; private set; }

    public void Update(string name, string serialNumber, string description,
        DateTime? purchaseDate, decimal? purchasePrice)
    {
        Name = name;
        Description = description;
        SerialNumber = serialNumber;
        PurchaseDate = purchaseDate;
        PurchasePrice = purchasePrice;
    }

    public void UpdateSupplier(Guid supplierId)
    {
        SupplierId = supplierId;
    }

    public void UpdateInvoice(Guid invoiceId)
    {
        PurchaseInvoiceId = invoiceId;
    }
}


