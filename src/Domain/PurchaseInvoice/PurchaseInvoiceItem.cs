namespace EdgIQ.MurimiOS.Domain.PurchaseInvoice;
using Tax = global::EdgIQ.MurimiOS.Domain.Tax.Tax;

public class PurchaseInvoiceItem(InvoicedItem invoicedItem, decimal unitPrice, int units, Guid? taxId) : BaseEntity
{
    public decimal UnitPrice { get; private set; } = unitPrice;

    public int Units { get; private set; } = units;

    public InvoicedItem InvoicedItem { get; private set; } = invoicedItem;

    public Guid? TaxId { get; private set; } = taxId;

    public Tax? Tax { get; private set; }
}


