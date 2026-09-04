using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;

/// <summary>
/// An immutable, effective-dated snapshot of a regulator or recording-organisation contract.
/// New legal rules and schemas are represented by a new version so historical submissions retain
/// the deadline and interchange contract under which they were created.
/// </summary>
public sealed class LivestockRegulatoryProfile : TenantEntity
{
    private LivestockRegulatoryProfile() { }

    public LivestockRegulatoryProfile(Guid id, string profileKey, int version, string name,
        LivestockRegulatoryExchangeType exchangeType, string jurisdiction, string species,
        string providerCode, string schemaVersion, string mediaType, int? reportingDeadlineHours,
        int? registerDeadlineHours, DateOnly effectiveFrom, DateOnly? effectiveTo, string sourceUri,
        bool isBuiltIn) : base(id)
    {
        if (!Enum.IsDefined(exchangeType)) throw new ArgumentOutOfRangeException(nameof(exchangeType));
        if (version <= 0 || reportingDeadlineHours is <= 0 or > 87_600
            || registerDeadlineHours is <= 0 or > 87_600 || effectiveTo < effectiveFrom)
            throw new ArgumentException("Profile version, deadlines, and effective dates are invalid.");
        if (exchangeType == LivestockRegulatoryExchangeType.Movement
            && (reportingDeadlineHours is null || registerDeadlineHours is null))
            throw new ArgumentException("Movement profiles require reporting and holding-register deadlines.");
        if (exchangeType != LivestockRegulatoryExchangeType.Movement && registerDeadlineHours is not null)
            throw new ArgumentException("Holding-register deadlines only apply to movement profiles.", nameof(registerDeadlineHours));
        if (sourceUri?.Length > 1_000 || !Uri.TryCreate(sourceUri, UriKind.Absolute, out Uri? source)
            || source.Scheme != Uri.UriSchemeHttps && source.Scheme != Uri.UriSchemeHttp)
            throw new ArgumentException("An absolute HTTP(S) source URI is required.", nameof(sourceUri));

        ProfileKey = Text(profileKey, nameof(profileKey), 120).ToLowerInvariant();
        ProfileVersion = version;
        Name = Text(name, nameof(name), 200);
        ExchangeType = exchangeType;
        Jurisdiction = Text(jurisdiction, nameof(jurisdiction), 20).ToUpperInvariant();
        Species = Text(species, nameof(species), 80).ToLowerInvariant();
        ProviderCode = Text(providerCode, nameof(providerCode), 80).ToLowerInvariant();
        SchemaVersion = Text(schemaVersion, nameof(schemaVersion), 40);
        MediaType = Text(mediaType, nameof(mediaType), 120).ToLowerInvariant();
        ReportingDeadlineHours = reportingDeadlineHours;
        RegisterDeadlineHours = registerDeadlineHours;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        SourceUri = source.AbsoluteUri;
        IsBuiltIn = isBuiltIn;
    }

    public string ProfileKey { get; private set; } = string.Empty;
    public int ProfileVersion { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public LivestockRegulatoryExchangeType ExchangeType { get; private set; }
    public string Jurisdiction { get; private set; } = string.Empty;
    public string Species { get; private set; } = string.Empty;
    public string ProviderCode { get; private set; } = string.Empty;
    public string SchemaVersion { get; private set; } = string.Empty;
    public string MediaType { get; private set; } = string.Empty;
    public int? ReportingDeadlineHours { get; private set; }
    public int? RegisterDeadlineHours { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string SourceUri { get; private set; } = string.Empty;
    public bool IsBuiltIn { get; private set; }

    public bool AppliesOn(DateTimeOffset eventAt) => AppliesOn(DateOnly.FromDateTime(eventAt.UtcDateTime));
    public bool AppliesOn(DateOnly date) => EffectiveFrom <= date && (EffectiveTo is null || EffectiveTo >= date);

    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum)
            throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }
}
