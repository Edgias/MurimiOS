namespace Edgias.MurimiOS.Domain.Entities;

public class PriceList(string name, decimal unitPrice,
        Guid itemId, Guid currencyId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public decimal UnitPrice { get; private set; } = unitPrice;

    public Guid ItemId { get; private set; } = itemId;

    public Item Item { get; private set; } = default!;

    public Guid CurrencyId { get; private set; } = currencyId;

    public Currency Currency { get; private set; } = default!;

    public void Update(string name, decimal unitPrice)
    {
        Name = name;
        UnitPrice = unitPrice;
    }

    public void UpdateCurrency(Guid currencyId)
    {
        CurrencyId = currencyId;
    }

}


