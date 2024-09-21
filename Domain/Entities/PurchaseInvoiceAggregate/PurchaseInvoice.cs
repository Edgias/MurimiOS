using Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;

namespace Edgias.MurimiOS.Domain.Entities.PurchaseInvoiceAggregate;

public class PurchaseInvoice(string name, DateTimeOffset dueDate, string invoiceNotes, 
    Guid? customerId, Guid? salesOrderId,
    Address billingAddress, List<PurchaseInvoiceItem> purchaseInvoiceItems) 
    : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public DateTimeOffset InvoiceDate { get; private set; } = DateTimeOffset.Now;

    public DateTimeOffset DueDate { get; private set; } = dueDate;

    public Address BillingAddress { get; private set; } = billingAddress;

    public string InvoiceNotes { get; private set; } = invoiceNotes;

    public Guid? SalesOrderId { get; private set; } = salesOrderId;

    public SalesOrder? SalesOrder { get; private set; }

    public Guid? CustomerId { get; private set; } = customerId;

    public Customer? Customer { get; private set; }

    private readonly List<PurchaseInvoiceItem> _purchaseInvoiceItems = purchaseInvoiceItems;

    public IReadOnlyCollection<PurchaseInvoiceItem> PurchaseInvoiceItems => _purchaseInvoiceItems.AsReadOnly();

    public void AddItem(PurchaseInvoiceItem purchaseInvoiceItem)
    {
        Guard.AgainstNull(purchaseInvoiceItem, nameof(purchaseInvoiceItem));

        _purchaseInvoiceItems.Add(purchaseInvoiceItem);
    }

    public decimal Total()
    {
        decimal total = 0m;

        foreach (PurchaseInvoiceItem purchaseInvoiceItem in _purchaseInvoiceItems)
        {
            decimal lineTotalBeforeTax = purchaseInvoiceItem.UnitPrice * purchaseInvoiceItem.Units;
            decimal lineTotalAfterTax = purchaseInvoiceItem.Tax != null ? 
                (lineTotalBeforeTax + (lineTotalBeforeTax * (purchaseInvoiceItem.Tax.Percentage / 100))) : 
                lineTotalBeforeTax;
            total += lineTotalAfterTax;
        }

        return total;
    }

    public decimal TotalTax()
    {
        decimal total = 0m;

        foreach (PurchaseInvoiceItem purchaseInvoiceItem in _purchaseInvoiceItems)
        {
            decimal lineTotalBeforeTax = purchaseInvoiceItem.UnitPrice * purchaseInvoiceItem.Units;
            decimal lineTax = purchaseInvoiceItem.Tax != null ? 
                (lineTotalBeforeTax * (purchaseInvoiceItem.Tax.Percentage / 100)) : 
                0;
            total += lineTax;
        }

        return total;
    }

    public decimal TotalWithoutTax()
    {
        decimal total = 0m;

        foreach (PurchaseInvoiceItem purchaseInvoiceItem in _purchaseInvoiceItems)
        {
            decimal lineTotal = purchaseInvoiceItem.UnitPrice * purchaseInvoiceItem.Units;
            total += lineTotal;
        }

        return total;
    }

}


