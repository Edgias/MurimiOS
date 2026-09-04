using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class BenchmarkEnrollment : TenantEntity
{
    private BenchmarkEnrollment() { }
    public BenchmarkEnrollment(Guid id, string metricCode, string cohortKey, int minimumCohortSize,
        DateTimeOffset enrolledAt, string enrolledBy, Guid? cohortDefinitionId = null) : base(id)
    {
        if (minimumCohortSize is < 5 or > 1_000 || cohortDefinitionId == Guid.Empty) throw new ArgumentOutOfRangeException(nameof(minimumCohortSize));
        MetricCode = KpiDefinition.Text(metricCode, 120).ToLowerInvariant(); CohortKey = KpiDefinition.Text(cohortKey, 120).ToLowerInvariant();
        MinimumCohortSize = minimumCohortSize; EnrolledAt = enrolledAt.ToUniversalTime(); EnrolledBy = KpiDefinition.Text(enrolledBy, 240); IsActive = true;
        CohortDefinitionId = cohortDefinitionId;
    }
    public string MetricCode { get; private set; } = string.Empty;
    public string CohortKey { get; private set; } = string.Empty;
    public int MinimumCohortSize { get; private set; }
    public DateTimeOffset EnrolledAt { get; private set; }
    public string EnrolledBy { get; private set; } = string.Empty;
    public Guid? CohortDefinitionId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }
    public void Revoke(DateTimeOffset at, string actor) { if (!IsActive) throw new InvalidOperationException("The benchmark enrollment is already revoked."); IsActive = false; RevokedAt = at.ToUniversalTime(); RevokedBy = KpiDefinition.Text(actor, 240); }
}
