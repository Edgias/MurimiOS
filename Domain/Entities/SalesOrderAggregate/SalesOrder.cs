using Edgias.MurimiOS.Domain.Entities.QuotationAggregate;

namespace Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;

public class SalesOrder(string name, Guid? customerId, Guid? quotationId, Address shipToAddress,
        List<SalesOrderItem> salesOrderItems) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.Now;

    public Address ShipToAddress { get; private set; } = shipToAddress;

    public Guid? CustomerId { get; private set; } = customerId;

    public Customer? Customer { get; private set; }

    public Guid? QuotationId { get; private set; } = quotationId;

    public Quotation? Quotation { get; private set; }

    private readonly List<SalesOrderItem> _salesOrderItems = salesOrderItems;
    public IReadOnlyCollection<SalesOrderItem> SalesOrderItems => _salesOrderItems.AsReadOnly();

    public void AddItem(SalesOrderItem salesOrderItem)
    {
        Guard.AgainstNull(salesOrderItem, nameof(salesOrderItem));

        _salesOrderItems.Add(salesOrderItem);
    }

    public decimal Total()
    {
        decimal total = 0m;

        foreach (SalesOrderItem salesOrderItem in _salesOrderItems)
        {
            total += salesOrderItem.UnitPrice * salesOrderItem.Units;
        }

        return total;
    }
}


