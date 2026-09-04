using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class AnalyticsFact : TenantEntity
{
    private AnalyticsFact() { }
    public AnalyticsFact(Guid id, Guid projectionRunId, Guid farmId, Guid organisationNodeId, AnalyticsFactKind kind,
        AnalyticsValueKind valueKind, string metricCode, decimal value, string unit, DateOnly periodStart, DateOnly periodEnd,
        decimal dataQualityPercent, int sourceRecordCount, string formulaVersion, string grainKey, Guid? seasonId = null,
        Guid? fieldId = null, Guid? cropZoneId = null, Guid? livestockSubjectId = null, Guid? supplierId = null,
        string? crop = null, string? variety = null, string? species = null, string? dimensionsJson = null,
        Guid? predictiveModelDefinitionId = null, decimal? confidencePercent = null,
        decimal? lowerBound = null, decimal? upperBound = null) : base(id)
    {
        if (projectionRunId == Guid.Empty || farmId == Guid.Empty || organisationNodeId == Guid.Empty || seasonId == Guid.Empty || fieldId == Guid.Empty || cropZoneId == Guid.Empty || livestockSubjectId == Guid.Empty || supplierId == Guid.Empty || predictiveModelDefinitionId == Guid.Empty)
            throw new ArgumentException("Projection, farm and organisation are required; optional dimensions must be non-empty.");
        if (!Enum.IsDefined(kind) || !Enum.IsDefined(valueKind) || periodEnd < periodStart || dataQualityPercent is < 0 or > 100 || sourceRecordCount < 0) throw new ArgumentException("Fact type, period, quality, and source count are invalid.");
        ProjectionRunId = projectionRunId; FarmId = farmId; OrganisationNodeId = organisationNodeId; Kind = kind; ValueKind = valueKind;
        MetricCode = KpiDefinition.Text(metricCode, 120).ToLowerInvariant(); Value = value; Unit = KpiDefinition.Text(unit, 40);
        PeriodStart = periodStart; PeriodEnd = periodEnd; DataQualityPercent = dataQualityPercent; SourceRecordCount = sourceRecordCount;
        FormulaVersion = KpiDefinition.Text(formulaVersion, 60); GrainKey = KpiDefinition.Text(grainKey, 300);
        SeasonId = seasonId; FieldId = fieldId; CropZoneId = cropZoneId; LivestockSubjectId = livestockSubjectId; SupplierId = supplierId;
        Crop = Optional(crop, 120); Variety = Optional(variety, 120); Species = Optional(species, 80); DimensionsJson = Optional(dimensionsJson, 8_000);
        if (confidencePercent is < 0 or > 100 || lowerBound > upperBound
            || valueKind != AnalyticsValueKind.Forecast && (predictiveModelDefinitionId is not null || confidencePercent is not null || lowerBound is not null || upperBound is not null))
            throw new ArgumentException("Prediction provenance and intervals are valid only for forecast facts.");
        PredictiveModelDefinitionId = predictiveModelDefinitionId; ConfidencePercent = confidencePercent;
        LowerBound = lowerBound; UpperBound = upperBound;
    }
    public Guid ProjectionRunId { get; private set; }
    public Guid FarmId { get; private set; }
    public Guid OrganisationNodeId { get; private set; }
    public AnalyticsFactKind Kind { get; private set; }
    public AnalyticsValueKind ValueKind { get; private set; }
    public string MetricCode { get; private set; } = string.Empty;
    public decimal Value { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateOnly PeriodStart { get; private set; }
    public DateOnly PeriodEnd { get; private set; }
    public decimal DataQualityPercent { get; private set; }
    public int SourceRecordCount { get; private set; }
    public string FormulaVersion { get; private set; } = string.Empty;
    public string GrainKey { get; private set; } = string.Empty;
    public Guid? SeasonId { get; private set; }
    public Guid? FieldId { get; private set; }
    public Guid? CropZoneId { get; private set; }
    public Guid? LivestockSubjectId { get; private set; }
    public Guid? SupplierId { get; private set; }
    public string? Crop { get; private set; }
    public string? Variety { get; private set; }
    public string? Species { get; private set; }
    public string? DimensionsJson { get; private set; }
    public Guid? PredictiveModelDefinitionId { get; private set; }
    public decimal? ConfidencePercent { get; private set; }
    public decimal? LowerBound { get; private set; }
    public decimal? UpperBound { get; private set; }
    private static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Text cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
