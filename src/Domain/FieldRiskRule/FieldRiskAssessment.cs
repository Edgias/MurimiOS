using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.FieldRiskRule;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;

public sealed class FieldRiskAssessment : TenantEntity
{
    private FieldRiskAssessment() { }
    public FieldRiskAssessment(Guid id, Guid farmId, Guid fieldId, Guid boundaryVersionId, Guid ruleId,
        int ruleVersion, DateTimeOffset assessedAt, DateTimeOffset windowStart, DateTimeOffset windowEnd,
        decimal? observedValue, int sampleCount, bool triggered, InsightSeverity severity,
        string evaluatorVersion, string inputSnapshotJson) : base(id)
    {
        if (new[] { farmId, fieldId, boundaryVersionId, ruleId }.Any(x => x == Guid.Empty) || ruleVersion <= 0 || windowEnd <= windowStart || sampleCount < 0 || !Enum.IsDefined(severity))
            throw new ArgumentException("Risk-assessment identity, window, or values are invalid.");
        if (triggered && observedValue is null) throw new ArgumentException("A triggered assessment requires an observed value.");
        FarmId = farmId; FieldId = fieldId; BoundaryVersionId = boundaryVersionId; RuleId = ruleId; RuleVersion = ruleVersion;
        AssessedAt = assessedAt.ToUniversalTime(); WindowStart = windowStart.ToUniversalTime(); WindowEnd = windowEnd.ToUniversalTime();
        ObservedValue = observedValue; SampleCount = sampleCount; Triggered = triggered; Severity = severity;
        EvaluatorVersion = Text(evaluatorVersion, nameof(evaluatorVersion), 40); InputSnapshotJson = Json(inputSnapshotJson, nameof(inputSnapshotJson), 32_000);
    }
    public Guid FarmId { get; private set; }
    public Guid FieldId { get; private set; }
    public Guid BoundaryVersionId { get; private set; }
    public Guid RuleId { get; private set; }
    public int RuleVersion { get; private set; }
    public DateTimeOffset AssessedAt { get; private set; }
    public DateTimeOffset WindowStart { get; private set; }
    public DateTimeOffset WindowEnd { get; private set; }
    public decimal? ObservedValue { get; private set; }
    public int SampleCount { get; private set; }
    public bool Triggered { get; private set; }
    public InsightSeverity Severity { get; private set; }
    public string EvaluatorVersion { get; private set; } = string.Empty;
    public string InputSnapshotJson { get; private set; } = "{}";
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string Json(string? value, string parameter, int maximum) => Text(value, parameter, maximum);
}
