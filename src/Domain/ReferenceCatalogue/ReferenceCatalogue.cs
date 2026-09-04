using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

public sealed class ReferenceCatalogue : TenantEntity
{
    private ReferenceCatalogue() { }

    public ReferenceCatalogue(
        Guid id,
        string key,
        string name,
        string version,
        string? countryCode,
        string? authority,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo = null) : base(id)
    {
        if (effectiveTo < effectiveFrom) throw new ArgumentException("Catalogue end date cannot precede its start date.", nameof(effectiveTo));
        Key = Require(key, nameof(key), 120).ToLowerInvariant();
        Name = Require(name, nameof(name), 200);
        CatalogueVersion = Require(version, nameof(version), 80);
        CountryCode = NormalizeCountry(countryCode);
        Authority = Optional(authority, 200);
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }

    public string Key { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string CatalogueVersion { get; private set; } = string.Empty;
    public string? CountryCode { get; private set; }
    public string? Authority { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }

    public bool IsEffectiveOn(DateOnly date) => EffectiveFrom <= date && (EffectiveTo is null || EffectiveTo >= date);

    private static string Require(string value, string parameter, int maximumLength)
    {
        string normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is 0 || normalized.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return normalized;
    }

    private static string? Optional(string? value, int maximumLength)
    {
        string? normalized = value?.Trim();
        if (normalized?.Length > maximumLength) throw new ArgumentException($"Value cannot exceed {maximumLength} characters.");
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }

    private static string? NormalizeCountry(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        string normalized = value.Trim().ToUpperInvariant();
        if (normalized.Length != 2 || normalized.Any(character => !char.IsAsciiLetter(character)))
            throw new ArgumentException("Catalogue country must be a two-letter ISO 3166-1 code.", nameof(value));
        return normalized;
    }
}

public sealed class ReferenceCatalogueItem : TenantEntity
{
    private ReferenceCatalogueItem() { }

    public ReferenceCatalogueItem(
        Guid id,
        Guid catalogueId,
        string code,
        string name,
        string? description,
        string attributesJson,
        DateOnly effectiveFrom,
        DateOnly? effectiveTo = null) : base(id)
    {
        if (catalogueId == Guid.Empty) throw new ArgumentException("A catalogue is required.", nameof(catalogueId));
        if (effectiveTo < effectiveFrom) throw new ArgumentException("Item end date cannot precede its start date.", nameof(effectiveTo));
        CatalogueId = catalogueId;
        Code = Require(code, nameof(code), 120).ToUpperInvariant();
        Name = Require(name, nameof(name), 200);
        Description = Optional(description, 2_000);
        AttributesJson = ValidateAttributes(attributesJson);
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }

    public Guid CatalogueId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string AttributesJson { get; private set; } = "{}";
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }

    public bool IsEffectiveOn(DateOnly date) => EffectiveFrom <= date && (EffectiveTo is null || EffectiveTo >= date);

    private static string ValidateAttributes(string value)
    {
        string normalized = string.IsNullOrWhiteSpace(value) ? "{}" : value.Trim();
        try
        {
            using JsonDocument document = JsonDocument.Parse(normalized);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw new ArgumentException("Catalogue item attributes must be a JSON object.", nameof(value));
            return document.RootElement.GetRawText();
        }
        catch (JsonException exception)
        {
            throw new ArgumentException("Catalogue item attributes must contain valid JSON.", nameof(value), exception);
        }
    }

    private static string Require(string value, string parameter, int maximumLength)
    {
        string normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length is 0 || normalized.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return normalized;
    }

    private static string? Optional(string? value, int maximumLength)
    {
        string? normalized = value?.Trim();
        if (normalized?.Length > maximumLength) throw new ArgumentException($"Value cannot exceed {maximumLength} characters.");
        return string.IsNullOrEmpty(normalized) ? null : normalized;
    }
}
