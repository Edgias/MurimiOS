using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.QualityAssessment;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class QualityAssessment : TenantEntity
{
    private QualityAssessment() { }
    public QualityAssessment(Guid id, TraceObjectType objectType, Guid objectId, string metric,
        decimal? numericResult, string? textResult, string? unit, bool passed, DateTimeOffset assessedAt, string? notes) : base(id)
    {
        if (objectId == Guid.Empty) throw new ArgumentException("An assessed object is required.", nameof(objectId));
        if (numericResult is null && string.IsNullOrWhiteSpace(textResult)) throw new ArgumentException("A numeric or text result is required.");
        if (numericResult is not null && !string.IsNullOrWhiteSpace(unit)) MeasurementUnits.Get(unit);
        ObjectType = objectType; ObjectId = objectId; Metric = Text(metric, nameof(metric), 120);
        NumericResult = numericResult; TextResult = Optional(textResult, 500); Unit = Optional(unit, 30);
        Passed = passed; AssessedAt = assessedAt; Notes = Optional(notes, 2_000);
    }
    public TraceObjectType ObjectType { get; private set; }
    public Guid ObjectId { get; private set; }
    public string Metric { get; private set; } = string.Empty;
    public decimal? NumericResult { get; private set; }
    public string? TextResult { get; private set; }
    public string? Unit { get; private set; }
    public bool Passed { get; private set; }
    public DateTimeOffset AssessedAt { get; private set; }
    public string? Notes { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
