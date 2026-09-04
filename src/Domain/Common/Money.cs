using System.Globalization;

namespace EdgIQ.MurimiOS.Domain.Common;

public readonly record struct CurrencyCode
{
    public CurrencyCode(string value)
    {
        string normalized = value?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalized.Length != 3 || normalized.Any(character => !char.IsAsciiLetter(character)))
            throw new ArgumentException("A three-letter ISO 4217 currency code is required.", nameof(value));
        Value = normalized;
    }

    public string Value { get; }
    public override string ToString() => Value;
}

public readonly record struct Money
{
    public Money(decimal amount, CurrencyCode currency)
    {
        if (string.IsNullOrEmpty(currency.Value)) throw new ArgumentException("A currency is required.", nameof(currency));
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public CurrencyCode Currency { get; }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal multiplier) => new(Amount * multiplier, Currency);

    public string Format(string locale)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(locale);
        return $"{Currency.Value} {Amount.ToString("N2", culture)}";
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot combine {Currency} and {other.Currency} money values.");
    }
}
