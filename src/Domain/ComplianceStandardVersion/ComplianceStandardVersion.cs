using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;

public sealed class ComplianceStandardVersion : TenantEntity
{
    private ComplianceStandardVersion() { }
    public ComplianceStandardVersion(Guid id, Guid seriesId, string code, string name, string scheme,
        string versionLabel, int revision, DateOnly effectiveFrom, DateOnly? effectiveTo, string? jurisdiction,
        string? packKey = null, string? sourceUri = null, string? manifestSha256 = null,
        string? conformancePolicyJson = null) : base(id)
    {
        if (seriesId == Guid.Empty) throw new ArgumentException("A standard series is required.", nameof(seriesId));
        if (revision <= 0 || effectiveTo < effectiveFrom) throw new ArgumentOutOfRangeException(nameof(revision));
        SeriesId = seriesId; Code = Text(code, 60); Name = Text(name, 200); Scheme = Text(scheme, 80);
        VersionLabel = Text(versionLabel, 60); Revision = revision; EffectiveFrom = effectiveFrom; EffectiveTo = effectiveTo;
        Jurisdiction = Optional(jurisdiction, 80); PackKey = Optional(packKey, 120)?.ToLowerInvariant();
        SourceUri = Optional(sourceUri, 1_000); ManifestSha256 = OptionalSha256(manifestSha256); Status = ComplianceStandardStatus.Draft;
        ConformancePolicyJson = JsonObject(conformancePolicyJson ?? "{\"minimumScorePercent\":80,\"criticalControlsMustConform\":true}");
    }
    public Guid SeriesId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Scheme { get; private set; } = string.Empty;
    public string VersionLabel { get; private set; } = string.Empty;
    public int Revision { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string? Jurisdiction { get; private set; }
    public string? PackKey { get; private set; }
    public string? SourceUri { get; private set; }
    public string? ManifestSha256 { get; private set; }
    public string ConformancePolicyJson { get; private set; } = "{}";
    public ComplianceStandardStatus Status { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public string? PublishedBy { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void Publish(DateTimeOffset at, string actor)
    {
        if (Status != ComplianceStandardStatus.Draft) throw new InvalidOperationException("Only a draft standard can be published.");
        Status = ComplianceStandardStatus.Published; PublishedAt = at.ToUniversalTime(); PublishedBy = Text(actor, 240);
    }
    public void Retire(DateTimeOffset at)
    {
        if (Status != ComplianceStandardStatus.Published) throw new InvalidOperationException("Only a published standard can be retired.");
        Status = ComplianceStandardStatus.Retired; RetiredAt = at.ToUniversalTime();
    }
    internal static string Text(string? value, int max) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > max) throw new ArgumentException($"Text must contain between 1 and {max} characters."); return result; }
    internal static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Text cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
    private static string? OptionalSha256(string? value)
    {
        string? result = Optional(value, 64)?.ToLowerInvariant();
        if (result is not null && (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x)))) throw new ArgumentException("A hexadecimal SHA-256 manifest digest is required.");
        return result;
    }
    private static string JsonObject(string value)
    {
        try
        {
            using System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(value);
            if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object) throw new ArgumentException("Conformance policy must be a JSON object.");
            return document.RootElement.GetRawText();
        }
        catch (System.Text.Json.JsonException exception) { throw new ArgumentException("Conformance policy must be valid JSON.", exception); }
    }
}
