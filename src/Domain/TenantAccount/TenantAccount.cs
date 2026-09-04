using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.TenantAccount;

public sealed class TenantAccount : TenantEntity
{
    private TenantAccount() { }

    public TenantAccount(Guid id, string legalName, string displayName, string slug, string countryCode,
        string billingCurrency, string timeZone, DateTimeOffset provisionedAt, string provisionedBy) : base(id)
    {
        LegalName = Text(legalName, 240);
        DisplayName = Text(displayName, 200);
        Slug = Code(slug, 80);
        CountryCode = Country(countryCode);
        BillingCurrency = Currency(billingCurrency);
        TimeZone = Text(timeZone, 80);
        ProvisionedAt = provisionedAt.ToUniversalTime();
        ProvisionedBy = Text(provisionedBy, 240);
        Status = TenantAccountStatus.Provisioning;
    }

    public string LegalName { get; private set; } = "";
    public string DisplayName { get; private set; } = "";
    public string Slug { get; private set; } = "";
    public string CountryCode { get; private set; } = "";
    public string BillingCurrency { get; private set; } = "";
    public string TimeZone { get; private set; } = "";
    public TenantAccountStatus Status { get; private set; }
    public DateTimeOffset ProvisionedAt { get; private set; }
    public string ProvisionedBy { get; private set; } = "";
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? SuspendedAt { get; private set; }
    public string? SuspensionReason { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }

    public void Activate(DateTimeOffset at)
    {
        if (Status is not (TenantAccountStatus.Provisioning or TenantAccountStatus.Suspended))
            throw new InvalidOperationException("Only a provisioning or suspended tenant can be activated.");
        Status = TenantAccountStatus.Active;
        ActivatedAt ??= at.ToUniversalTime();
        SuspendedAt = null;
        SuspensionReason = null;
    }

    public void Suspend(DateTimeOffset at, string reason)
    {
        if (Status != TenantAccountStatus.Active) throw new InvalidOperationException("Only an active tenant can be suspended.");
        Status = TenantAccountStatus.Suspended;
        SuspendedAt = at.ToUniversalTime();
        SuspensionReason = Text(reason, 1_000);
    }

    public void Close(DateTimeOffset at, string reason)
    {
        if (Status == TenantAccountStatus.Closed) throw new InvalidOperationException("Tenant is already closed.");
        Status = TenantAccountStatus.Closed;
        ClosedAt = at.ToUniversalTime();
        SuspensionReason = Text(reason, 1_000);
    }

    internal static string Text(string? value, int maximum)
    {
        string result = value?.Trim() ?? "";
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"Value must contain between 1 and {maximum} characters.");
        return result;
    }

    internal static string Optional(string? value, int maximum)
    {
        string result = value?.Trim() ?? "";
        if (result.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters.");
        return result;
    }

    internal static string Code(string value, int maximum)
    {
        string result = Text(value, maximum).ToLowerInvariant();
        if (result.Any(character => !(char.IsAsciiLetterOrDigit(character) || character is '-' or '_' or '.')))
            throw new ArgumentException("Codes may contain only ASCII letters, numbers, hyphens, underscores, and periods.");
        return result;
    }

    internal static string Country(string value)
    {
        string result = Text(value, 2).ToUpperInvariant();
        if (result.Length != 2 || result.Any(character => !char.IsAsciiLetter(character))) throw new ArgumentException("Country code must use ISO alpha-2 format.");
        return result;
    }

    internal static string Currency(string value)
    {
        string result = Text(value, 3).ToUpperInvariant();
        if (result.Length != 3 || result.Any(character => !char.IsAsciiLetter(character))) throw new ArgumentException("Currency must use ISO alpha-3 format.");
        return result;
    }
}
