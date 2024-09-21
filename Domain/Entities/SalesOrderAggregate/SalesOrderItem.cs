namespace Edgias.MurimiOS.Domain.Entities.SalesOrderAggregate;

public class SalesOrderItem(ItemOrdered itemOrdered, decimal unitPrice, int units) : BaseEntity
{
    public ItemOrdered ItemOrdered { get; private set; } = itemOrdered;

    public decimal UnitPrice { get; private set; } = unitPrice;

    public int Units { get; private set; } = units;
}


