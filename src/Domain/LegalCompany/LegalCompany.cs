using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LegalCompany;

public sealed class LegalCompany : TenantEntity
{
    private LegalCompany() { }

    public LegalCompany(Guid id, Guid organisationNodeId, string companyCode, string legalName,
        string registrationNumber, string? taxIdentifier, string countryCode, string functionalCurrency,
        DateOnly effectiveFrom, string createdBy) : base(id)
    {
        if (organisationNodeId == Guid.Empty) throw new ArgumentException("An organisation node is required.");
        string country = Required(countryCode, 2).ToUpperInvariant();
        if (country.Length != 2 || !country.All(char.IsLetter)) throw new ArgumentException("A two-letter country code is required.");
        OrganisationNodeId = organisationNodeId;
        CompanyCode = Required(companyCode, 80).ToUpperInvariant();
        LegalName = Required(legalName, 240);
        RegistrationNumber = Required(registrationNumber, 120).ToUpperInvariant();
        TaxIdentifier = Optional(taxIdentifier, 120)?.ToUpperInvariant();
        CountryCode = country;
        FunctionalCurrency = new CurrencyCode(functionalCurrency).Value;
        EffectiveFrom = effectiveFrom;
        CreatedBy = Required(createdBy, 240);
        IsActive = true;
    }

    public Guid OrganisationNodeId { get; private set; }
    public string CompanyCode { get; private set; } = string.Empty;
    public string LegalName { get; private set; } = string.Empty;
    public string RegistrationNumber { get; private set; } = string.Empty;
    public string? TaxIdentifier { get; private set; }
    public string CountryCode { get; private set; } = string.Empty;
    public string FunctionalCurrency { get; private set; } = string.Empty;
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }

    public void Close(DateOnly effectiveTo, DateTimeOffset closedAt, string actor)
    {
        if (!IsActive || effectiveTo < EffectiveFrom) throw new InvalidOperationException("Only an active company can close on or after its effective date.");
        IsActive = false; EffectiveTo = effectiveTo; ClosedAt = closedAt.ToUniversalTime(); ClosedBy = Required(actor, 240);
    }

    internal static string Required(string? value, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length == 0 || result.Length > maximum) throw new ArgumentException($"Text must contain between 1 and {maximum} characters.");
        return result;
    }

    internal static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Text cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
