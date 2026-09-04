using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.FieldMapLayer;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;

public sealed class FieldMapLayer : TenantEntity
{
    private FieldMapLayer() { }
    public FieldMapLayer(Guid id, Guid farmId, Guid fieldId, Guid boundaryVersionId, MapLayerKind kind,
        string name, string sourceType, Guid sourceId, DateTimeOffset sourceObservedAt, DateTimeOffset generatedAt,
        DateTimeOffset validFrom, DateTimeOffset? validTo, string geoJson, string styleJson, InsightQuality quality) : base(id)
    {
        if (new[] { farmId, fieldId, boundaryVersionId, sourceId }.Any(x => x == Guid.Empty) || validTo <= validFrom || !Enum.IsDefined(kind) || !Enum.IsDefined(quality)) throw new ArgumentException("Map-layer identity, period, or quality is invalid.");
        FarmId = farmId; FieldId = fieldId; BoundaryVersionId = boundaryVersionId; Kind = kind; Name = Text(name, nameof(name), 200);
        SourceType = Text(sourceType, nameof(sourceType), 100); SourceId = sourceId; SourceObservedAt = sourceObservedAt.ToUniversalTime(); GeneratedAt = generatedAt.ToUniversalTime();
        ValidFrom = validFrom.ToUniversalTime(); ValidTo = validTo?.ToUniversalTime(); GeoJson = Text(geoJson, nameof(geoJson), 1_000_000); StyleJson = Text(styleJson, nameof(styleJson), 32_000); Quality = quality;
    }
    public Guid FarmId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid BoundaryVersionId { get; private set; }
    public MapLayerKind Kind { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string SourceType { get; private set; } = string.Empty;
    public Guid SourceId { get; private set; }
    public DateTimeOffset SourceObservedAt { get; private set; }
    public DateTimeOffset GeneratedAt { get; private set; }
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset? ValidTo { get; private set; }
    public string GeoJson { get; private set; } = "{}";
    public string StyleJson { get; private set; } = "{}";
    public InsightQuality Quality { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}
