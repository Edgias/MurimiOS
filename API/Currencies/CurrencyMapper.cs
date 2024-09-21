namespace Edgias.MurimiOS.API.Currencies;

public static class CurrencyMapper
{
    public static CurrencyResponse AsApiResponse(this Currency currency)
    {
        CurrencyResponse dto = new()
        {
            Id = currency.Id,
            Name = currency.Name,
            Symbol = currency.Symbol,
            CurrencyCode = currency.CurrencyCode
        };

        return dto;
    }

    public static Currency ToEntity(this CurrencyRequest request)
    {
        Currency currency = new(request.Name, request.CurrencyCode, request.Symbol);

        return currency;
    }

    public static void Update(this Currency currency, CurrencyRequest request)
    {
        currency.Update(request.Name, request.CurrencyCode, request.Symbol);
    }
}
