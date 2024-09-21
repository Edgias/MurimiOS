namespace Edgias.MurimiOS.Domain.Entities.QuotationAggregate;

public class Quotation : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;

    public DateTimeOffset QuotationDate { get; private set; }

    public DateTimeOffset? ExpiryDate { get; private set; }

    public Guid? CustomerId { get; private set; }

    public Customer? Customer { get; set; }

    private readonly List<QuotationItem> _quotationItems = [];

    public Quotation(string name, DateTimeOffset quotationDate, DateTimeOffset? expiryDate, Guid? customerId)
    {
        Name = name;
        QuotationDate = quotationDate;
        ExpiryDate = expiryDate;
        CustomerId = customerId;
    }

    public Quotation(string name, DateTimeOffset quotationDate, DateTimeOffset? expiryDate,
        List<QuotationItem> quotationItems, Guid? customerId)
    {
        Name = name;
        QuotationDate = quotationDate;
        ExpiryDate = expiryDate;
        CustomerId = customerId;
        _quotationItems = quotationItems;
    }

    public IReadOnlyCollection<QuotationItem> QuotationItems => _quotationItems.AsReadOnly();

    public void AddItem(QuotationItem quotationItem)
    {
        Guard.AgainstNull(quotationItem, nameof(quotationItem));

        _quotationItems.Add(quotationItem);
    }

    public decimal Total()
    {
        decimal total = 0m;

        foreach (QuotationItem quotationItem in _quotationItems)
        {
            decimal lineTotalBeforeTax = quotationItem.UnitPrice * quotationItem.Units;
            decimal lineTotalAfterTax = quotationItem.Tax != null ?
                (lineTotalBeforeTax + (lineTotalBeforeTax * (quotationItem.Tax!.Percentage / 100))) :
                lineTotalBeforeTax;

            total += lineTotalAfterTax;
        }

        return total;
    }

    public decimal TotalTax()
    {
        decimal total = 0m;

        foreach (QuotationItem quotationItem in _quotationItems)
        {
            decimal lineTotalBeforeTax = quotationItem.UnitPrice * quotationItem.Units;

            decimal lineTax = quotationItem.Tax != null ? (lineTotalBeforeTax * (quotationItem.Tax!.Percentage / 100)) : 0;

            total += lineTax;
        }

        return total;
    }

    public decimal TotalWithoutTax()
    {
        decimal total = 0m;

        foreach (QuotationItem quotationItem in _quotationItems)
        {
            decimal lineTotal = quotationItem.UnitPrice * quotationItem.Units;
            total += lineTotal;
        }

        return total;
    }

    public void ChangeExpiryDate(DateTimeOffset expiryDate)
    {
        ExpiryDate = expiryDate;
    }

}


