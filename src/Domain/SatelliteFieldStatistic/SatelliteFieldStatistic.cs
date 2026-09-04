using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;

public sealed class SatelliteFieldStatistic : TenantEntity
{
    private SatelliteFieldStatistic() { }
    public SatelliteFieldStatistic(Guid id, Guid farmId, Guid fieldId, Guid boundaryVersionId, Guid feedId,
        Guid ingestionRecordId, string providerAcquisitionId, VegetationIndex index, string? customIndex,
        DateTimeOffset acquiredAt, DateTimeOffset processedAt, decimal mean, decimal minimum, decimal maximum,
        decimal standardDeviation, decimal validPixelPercent, decimal cloudPercent, InsightQuality quality) : base(id)
    {
        if (new[] { farmId, fieldId, boundaryVersionId, feedId, ingestionRecordId }.Any(x => x == Guid.Empty)) throw new ArgumentException("Required identifiers must be non-empty.");
        if (!Enum.IsDefined(index) || !Enum.IsDefined(quality) || minimum > mean || mean > maximum || standardDeviation < 0
            || validPixelPercent is < 0 or > 100 || cloudPercent is < 0 or > 100) throw new ArgumentException("Satellite statistic values are invalid.");
        if (index == VegetationIndex.Other && string.IsNullOrWhiteSpace(customIndex)) throw new ArgumentException("A custom index name is required.", nameof(customIndex));
        FarmId = farmId; FieldId = fieldId; BoundaryVersionId = boundaryVersionId; FeedId = feedId; IngestionRecordId = ingestionRecordId;
        ProviderAcquisitionId = Text(providerAcquisitionId, nameof(providerAcquisitionId), 240); Index = index;
        CustomIndex = Optional(customIndex, 40)?.ToUpperInvariant(); AcquiredAt = acquiredAt.ToUniversalTime(); ProcessedAt = processedAt.ToUniversalTime();
        Mean = mean; Minimum = minimum; Maximum = maximum; StandardDeviation = standardDeviation;
        ValidPixelPercent = validPixelPercent; CloudPercent = cloudPercent; Quality = quality;
    }
    public Guid FarmId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid BoundaryVersionId { get; private set; }
    public Guid FeedId { get; private set; }
    public Guid IngestionRecordId { get; private set; }
    public string ProviderAcquisitionId { get; private set; } = string.Empty;
    public VegetationIndex Index { get; private set; }
    public string? CustomIndex { get; private set; }
    public DateTimeOffset AcquiredAt { get; private set; }
    public DateTimeOffset ProcessedAt { get; private set; }
    public decimal Mean { get; private set; }
    public decimal Minimum { get; private set; }
    public decimal Maximum { get; private set; }
    public decimal StandardDeviation { get; private set; }
    public decimal ValidPixelPercent { get; private set; }
    public decimal CloudPercent { get; private set; }
    public InsightQuality Quality { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
