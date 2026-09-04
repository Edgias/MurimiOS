using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.FieldRiskRule;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;

public sealed class FieldRiskRule : TenantEntity
{
    private FieldRiskRule() { }
    public FieldRiskRule(Guid id, string ruleKey, int ruleVersion, string name, string? crop,
        RiskMetric metric, RiskAggregation aggregation, RiskComparison comparison,
        ScoutingCategory? scoutingCategory, string? issueCode, decimal threshold,
        int lookbackHours, int minimumSamples, InsightSeverity severity, string message,
        DateTimeOffset effectiveFrom, DateTimeOffset? effectiveTo, string createdBy) : base(id)
    {
        if (ruleVersion <= 0 || lookbackHours is < 1 or > 8_760 || minimumSamples is < 1 or > 100_000 || effectiveTo <= effectiveFrom)
            throw new ArgumentException("Risk-rule version, evaluation window, sample count, or effective period is invalid.");
        if (!Enum.IsDefined(metric) || !Enum.IsDefined(aggregation) || !Enum.IsDefined(comparison) || !Enum.IsDefined(severity)) throw new ArgumentOutOfRangeException(nameof(metric));
        if (scoutingCategory is { } category && !Enum.IsDefined(category)) throw new ArgumentOutOfRangeException(nameof(scoutingCategory));
        if ((scoutingCategory is not null || !string.IsNullOrWhiteSpace(issueCode)) && metric != RiskMetric.ScoutingSeverity)
            throw new ArgumentException("Scouting category and issue filters are only valid for scouting-severity rules.");
        RuleKey = Text(ruleKey, nameof(ruleKey), 120).ToLowerInvariant(); RuleVersion = ruleVersion; Name = Text(name, nameof(name), 200);
        Crop = Optional(crop, 120)?.ToLowerInvariant(); Metric = metric; Aggregation = aggregation; Comparison = comparison;
        ScoutingCategory = scoutingCategory; IssueCode = Optional(issueCode, 80)?.ToLowerInvariant(); Threshold = threshold;
        LookbackHours = lookbackHours; MinimumSamples = minimumSamples; Severity = severity; Message = Text(message, nameof(message), 1_000);
        EffectiveFrom = effectiveFrom.ToUniversalTime(); EffectiveTo = effectiveTo?.ToUniversalTime(); CreatedBy = Text(createdBy, nameof(createdBy), 240);
        Status = RiskRuleStatus.Draft;
    }
    public string RuleKey { get; private set; } = string.Empty;
    public int RuleVersion { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Crop { get; private set; }
    public RiskMetric Metric { get; private set; }
    public RiskAggregation Aggregation { get; private set; }
    public RiskComparison Comparison { get; private set; }
    public ScoutingCategory? ScoutingCategory { get; private set; }
    public string? IssueCode { get; private set; }
    public decimal Threshold { get; private set; }
    public int LookbackHours { get; private set; }
    public int MinimumSamples { get; private set; }
    public InsightSeverity Severity { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public DateTimeOffset EffectiveFrom { get; private set; }
    public DateTimeOffset? EffectiveTo { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public RiskRuleStatus Status { get; private set; }
    public DateTimeOffset? ActivatedAt { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void Activate(DateTimeOffset at) { if (Status != RiskRuleStatus.Draft) throw new InvalidOperationException("Only a draft rule can be activated."); Status = RiskRuleStatus.Active; ActivatedAt = at.ToUniversalTime(); }
    public void Retire(DateTimeOffset at) { if (Status != RiskRuleStatus.Active) throw new InvalidOperationException("Only an active rule can be retired."); Status = RiskRuleStatus.Retired; RetiredAt = at.ToUniversalTime(); }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
