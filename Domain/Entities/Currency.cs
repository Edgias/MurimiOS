namespace Edgias.MurimiOS.Domain.Entities;

public class Currency(string name, string currencyCode, string symbol) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string CurrencyCode { get; private set; } = currencyCode;

    public string Symbol { get; private set; } = symbol;

    public void Update(string name, string currencyCode, string symbol)
    {
        Name = name;
        CurrencyCode = currencyCode;
        Symbol = symbol;
    }

}


