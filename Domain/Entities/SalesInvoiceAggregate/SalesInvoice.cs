using Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;

namespace Edgias.MurimiOS.Domain.Entities.SalesInvoiceAggregate;

public class SalesInvoice : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }

    public DateTime InvoiceDate { get; private set; } = DateTime.UtcNow;

    public DateTime DueDate { get; private set; }

    public Address BillingAddress { get; private set; }

    public string InvoiceNotes { get; private set; }

    public Guid? SalesOrderId { get; private set; }

    public SalesOrder? SalesOrder { get; private set; }

    public Guid? CustomerId { get; private set; }

    public Customer? Customer { get; private set; }

    private readonly List<SalesInvoiceItem> _salesInvoiceItems = [];
    public IReadOnlyCollection<SalesInvoiceItem> SalesInvoiceItems => _salesInvoiceItems.AsReadOnly();

    public SalesInvoice(string name, DateTime dueDate, string invoiceNotes, Guid? customerId, Guid? salesOrderId,
            Address billingAddress)
    {
        Name = name;
        DueDate = dueDate;
        BillingAddress = billingAddress;
        InvoiceNotes = invoiceNotes;
        SalesOrderId = salesOrderId;
        CustomerId = customerId;
    }

    public SalesInvoice(string name, DateTime dueDate, string invoiceNotes, Guid? customerId, Guid? salesOrderId,
            Address billingAddress, List<SalesInvoiceItem> salesInvoiceItems)
    {
        Name = name;
        DueDate = dueDate;
        BillingAddress = billingAddress;
        InvoiceNotes = invoiceNotes;
        SalesOrderId = salesOrderId;
        CustomerId = customerId;
        _salesInvoiceItems = salesInvoiceItems;
    }

    public void AddItem(SalesInvoiceItem salesInvoiceItem)
    {
        Guard.AgainstNull(salesInvoiceItem, nameof(salesInvoiceItem));

        _salesInvoiceItems.Add(salesInvoiceItem);
    }

    public decimal Total()
    {
        decimal total = 0m;

        foreach (SalesInvoiceItem invoiceLine in _salesInvoiceItems)
        {
            decimal lineTotalBeforeTax = invoiceLine.UnitPrice * invoiceLine.Units;
            decimal lineTotalAfterTax = invoiceLine.Tax != null ?
                (lineTotalBeforeTax + (lineTotalBeforeTax * (invoiceLine.Tax.Percentage / 100))) :
                lineTotalBeforeTax;

            total += lineTotalAfterTax;
        }

        return total;
    }

    public decimal TotalTax()
    {
        decimal total = 0m;

        foreach (SalesInvoiceItem invoiceLine in _salesInvoiceItems)
        {
            decimal lineTotalBeforeTax = invoiceLine.UnitPrice * invoiceLine.Units;
            decimal lineTax = invoiceLine.Tax != null ?
                (lineTotalBeforeTax * (invoiceLine.Tax.Percentage / 100)) :
                0;

            total += lineTax;
        }

        return total;
    }

    public decimal TotalWithoutTax()
    {
        decimal total = 0m;

        foreach (SalesInvoiceItem invoiceLine in _salesInvoiceItems)
        {
            decimal lineTotal = invoiceLine.UnitPrice * invoiceLine.Units;
            total += lineTotal;
        }

        return total;
    }

}


