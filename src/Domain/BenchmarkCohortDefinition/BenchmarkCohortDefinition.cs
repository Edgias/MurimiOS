using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class BenchmarkCohortDefinition : TenantEntity
{
    private BenchmarkCohortDefinition() { }
    public BenchmarkCohortDefinition(Guid id, string cohortKey, int definitionVersion, string name,
        string description, string inclusionCriteriaJson, int minimumCohortSize, DateOnly effectiveFrom,
        DateOnly? effectiveTo, string owner, string createdBy) : base(id)
    {
        if (definitionVersion <= 0 || minimumCohortSize is < 5 or > 1_000 || effectiveTo < effectiveFrom)
            throw new ArgumentException("Cohort version, privacy threshold, or effective dates are invalid.");
        CohortKey = KpiDefinition.Text(cohortKey, 120).ToLowerInvariant();
        DefinitionVersion = definitionVersion;
        Name = KpiDefinition.Text(name, 200);
        Description = KpiDefinition.Text(description, 2_000);
        InclusionCriteriaJson = Json(inclusionCriteriaJson, 16_000);
        MinimumCohortSize = minimumCohortSize;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Owner = KpiDefinition.Text(owner, 240);
        CreatedBy = KpiDefinition.Text(createdBy, 240);
        Status = AnalyticsGovernanceStatus.Draft;
    }
    public string CohortKey { get; private set; } = string.Empty;
    public int DefinitionVersion { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string InclusionCriteriaJson { get; private set; } = "{}";
    public int MinimumCohortSize { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;
    public AnalyticsGovernanceStatus Status { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalReference { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public string? RetiredBy { get; private set; }
    public void Approve(DateTimeOffset at, string specialist, string approvalReference)
    {
        if (Status != AnalyticsGovernanceStatus.Draft) throw new InvalidOperationException("Only a draft cohort definition can be approved.");
        Status = AnalyticsGovernanceStatus.Active; ApprovedAt = at.ToUniversalTime();
        ApprovedBy = KpiDefinition.Text(specialist, 240); ApprovalReference = KpiDefinition.Text(approvalReference, 240);
    }
    public void Retire(DateTimeOffset at, string actor)
    {
        if (Status != AnalyticsGovernanceStatus.Active) throw new InvalidOperationException("Only an active cohort definition can be retired.");
        Status = AnalyticsGovernanceStatus.Retired; RetiredAt = at.ToUniversalTime(); RetiredBy = KpiDefinition.Text(actor, 240);
    }
    public bool AppliesOn(DateOnly date) => Status == AnalyticsGovernanceStatus.Active
        && EffectiveFrom <= date && (EffectiveTo is null || EffectiveTo >= date);
    private static string Json(string value, int max)
    {
        string result = KpiDefinition.Text(value, max); using JsonDocument _ = JsonDocument.Parse(result); return result;
    }
}
