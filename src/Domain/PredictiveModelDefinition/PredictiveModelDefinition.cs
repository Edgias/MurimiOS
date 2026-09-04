using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class PredictiveModelDefinition : TenantEntity
{
    private PredictiveModelDefinition() { }
    public PredictiveModelDefinition(Guid id, string modelKey, int modelVersion, string name,
        string targetMetricCode, string algorithm, string featureSchemaJson, int trainingWindowDays,
        int minimumObservations, decimal validationMae, decimal? validationMapePercent,
        string validationDatasetReference, string createdBy) : base(id)
    {
        if (modelVersion <= 0 || trainingWindowDays is < 30 or > 3_650 || minimumObservations is < 3 or > 10_000
            || validationMae < 0 || validationMapePercent is < 0 or > 1_000)
            throw new ArgumentException("Model version, training window, observations, or validation metrics are invalid.");
        ModelKey = KpiDefinition.Text(modelKey, 120).ToLowerInvariant(); ModelVersion = modelVersion;
        Name = KpiDefinition.Text(name, 200); TargetMetricCode = KpiDefinition.Text(targetMetricCode, 120).ToLowerInvariant();
        Algorithm = KpiDefinition.Text(algorithm, 120).ToLowerInvariant(); FeatureSchemaJson = Json(featureSchemaJson, 16_000);
        TrainingWindowDays = trainingWindowDays; MinimumObservations = minimumObservations;
        ValidationMae = validationMae; ValidationMapePercent = validationMapePercent;
        ValidationDatasetReference = KpiDefinition.Text(validationDatasetReference, 500);
        CreatedBy = KpiDefinition.Text(createdBy, 240); Status = AnalyticsGovernanceStatus.Draft;
    }
    public string ModelKey { get; private set; } = string.Empty;
    public int ModelVersion { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string TargetMetricCode { get; private set; } = string.Empty;
    public string Algorithm { get; private set; } = string.Empty;
    public string FeatureSchemaJson { get; private set; } = "{}";
    public int TrainingWindowDays { get; private set; }
    public int MinimumObservations { get; private set; }
    public decimal ValidationMae { get; private set; }
    public decimal? ValidationMapePercent { get; private set; }
    public string ValidationDatasetReference { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;
    public AnalyticsGovernanceStatus Status { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public string? ApprovedBy { get; private set; }
    public string? ApprovalReference { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public string? RetiredBy { get; private set; }
    public void Approve(DateTimeOffset at, string specialist, string approvalReference)
    {
        if (Status != AnalyticsGovernanceStatus.Draft) throw new InvalidOperationException("Only a draft predictive model can be approved.");
        Status = AnalyticsGovernanceStatus.Active; ApprovedAt = at.ToUniversalTime();
        ApprovedBy = KpiDefinition.Text(specialist, 240); ApprovalReference = KpiDefinition.Text(approvalReference, 240);
    }
    public void Retire(DateTimeOffset at, string actor)
    {
        if (Status != AnalyticsGovernanceStatus.Active) throw new InvalidOperationException("Only an active predictive model can be retired.");
        Status = AnalyticsGovernanceStatus.Retired; RetiredAt = at.ToUniversalTime(); RetiredBy = KpiDefinition.Text(actor, 240);
    }
    private static string Json(string value, int max)
    {
        string result = KpiDefinition.Text(value, max); using JsonDocument _ = JsonDocument.Parse(result); return result;
    }
}
