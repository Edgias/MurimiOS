using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;

public enum OpeningBalanceDirection { Debit = 1, Credit = 2 }

public sealed class FinanceOpeningBalance : TenantEntity
{
    private FinanceOpeningBalance() { }

    public FinanceOpeningBalance(Guid id, Guid farmId, string accountCode, string accountName,
        OpeningBalanceDirection direction, decimal amount, string currency, DateOnly asOf,
        string reference, DateTimeOffset importedAt, string importedBy) : base(id)
    {
        if (farmId == Guid.Empty || !Enum.IsDefined(direction) || amount <= 0)
            throw new ArgumentException("Opening-balance scope, direction, or amount is invalid.");
        FarmId = farmId;
        AccountCode = Required(accountCode, 80).ToUpperInvariant();
        AccountName = Required(accountName, 200);
        Direction = direction;
        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
        Currency = new CurrencyCode(currency).Value;
        AsOf = asOf;
        Reference = Required(reference, 160);
        ImportedAt = importedAt.ToUniversalTime();
        ImportedBy = Required(importedBy, 240);
    }

    public Guid FarmId { get; private set; }
    public string AccountCode { get; private set; } = "";
    public string AccountName { get; private set; } = "";
    public OpeningBalanceDirection Direction { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "";
    public DateOnly AsOf { get; private set; }
    public string Reference { get; private set; } = "";
    public DateTimeOffset ImportedAt { get; private set; }
    public string ImportedBy { get; private set; } = "";

    private static string Required(string? value, int maximum)
    {
        string result = value?.Trim() ?? "";
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"Text must contain 1-{maximum} characters.");
        return result;
    }
}
