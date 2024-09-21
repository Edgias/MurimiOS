namespace Edgias.MurimiOS.Domain.Entities.QuotationAggregate;

public class QuotationItem(ItemQuoted itemQuoted, decimal unitPrice, 
    int units, Guid? taxId) : BaseEntity
{
    public decimal UnitPrice { get; private set; } = unitPrice;

    public int Units { get; private set; } = units;

    public ItemQuoted ItemQuoted { get; private set; } = itemQuoted;

    public Guid? TaxId { get; private set; } = taxId;

    public Tax? Tax { get; private set; }

    public void ChangeQuantity(int units)
    {
        Guard.AgainstZero(units, nameof(units));

        Units = units;
    }

    public void ChangeUnitPrice(decimal unitPrice)
    {
        Guard.AgainstZero(unitPrice, nameof(unitPrice));

        UnitPrice = unitPrice;
    }
}


