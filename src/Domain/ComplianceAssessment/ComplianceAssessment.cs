using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ComplianceAssessment : TenantEntity
{
    private ComplianceAssessment() { }
    public ComplianceAssessment(Guid id, Guid farmId, Guid standardVersionId, string assessmentCode,
        DateOnly scheduledOn, string? auditor, string? scopeJson) : base(id)
    {
        if (farmId == Guid.Empty || standardVersionId == Guid.Empty) throw new ArgumentException("Farm and standard are required.");
        FarmId = farmId; StandardVersionId = standardVersionId; AssessmentCode = ComplianceStandardVersion.Text(assessmentCode, 80);
        ScheduledOn = scheduledOn; Auditor = ComplianceStandardVersion.Optional(auditor, 200);
        ScopeJson = ComplianceStandardVersion.Optional(scopeJson, 8_000); Status = ComplianceAssessmentStatus.Draft;
    }
    public Guid FarmId { get; private set; }
    public Guid StandardVersionId { get; private set; }
    public string AssessmentCode { get; private set; } = string.Empty;
    public DateOnly ScheduledOn { get; private set; }
    public string? Auditor { get; private set; }
    public string? ScopeJson { get; private set; }
    public ComplianceAssessmentStatus Status { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public decimal? ScorePercent { get; private set; }
    public bool? Conforms { get; private set; }
    public void Start(DateTimeOffset at) { if (Status != ComplianceAssessmentStatus.Draft) throw new InvalidOperationException("Only a draft assessment can start."); Status = ComplianceAssessmentStatus.InProgress; StartedAt = at.ToUniversalTime(); }
    public void Submit(DateTimeOffset at) { if (Status != ComplianceAssessmentStatus.InProgress) throw new InvalidOperationException("Only an in-progress assessment can be submitted."); Status = ComplianceAssessmentStatus.Submitted; SubmittedAt = at.ToUniversalTime(); }
    public void Complete(decimal scorePercent, bool conforms, DateTimeOffset at) { if (Status != ComplianceAssessmentStatus.Submitted) throw new InvalidOperationException("Only a submitted assessment can be completed."); if (scorePercent is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(scorePercent)); Status = ComplianceAssessmentStatus.Completed; ScorePercent = scorePercent; Conforms = conforms; CompletedAt = at.ToUniversalTime(); }
    public void Cancel(DateTimeOffset at) { if (Status is ComplianceAssessmentStatus.Completed or ComplianceAssessmentStatus.Cancelled) throw new InvalidOperationException("The assessment is already closed."); Status = ComplianceAssessmentStatus.Cancelled; CancelledAt = at.ToUniversalTime(); }
}
