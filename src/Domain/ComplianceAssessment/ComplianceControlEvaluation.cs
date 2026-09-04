using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ComplianceControlEvaluation : TenantEntity
{
    private ComplianceControlEvaluation() { }
    public ComplianceControlEvaluation(Guid id, Guid assessmentId, Guid controlPointId, ComplianceEvaluationResult result,
        decimal scorePercent, string evaluatorVersion, string evaluatedBy, DateTimeOffset evaluatedAt, string? notes,
        string evidenceSnapshotJson) : base(id)
    {
        if (assessmentId == Guid.Empty || controlPointId == Guid.Empty) throw new ArgumentException("Assessment and control are required.");
        if (!Enum.IsDefined(result) || scorePercent is < 0 or > 100 || result == ComplianceEvaluationResult.NotApplicable && scorePercent != 0) throw new ArgumentOutOfRangeException(nameof(scorePercent));
        AssessmentId = assessmentId; ControlPointId = controlPointId; Result = result; ScorePercent = scorePercent;
        EvaluatorVersion = ComplianceStandardVersion.Text(evaluatorVersion, 60); EvaluatedBy = ComplianceStandardVersion.Text(evaluatedBy, 240);
        EvaluatedAt = evaluatedAt.ToUniversalTime(); Notes = ComplianceStandardVersion.Optional(notes, 4_000);
        EvidenceSnapshotJson = ComplianceStandardVersion.Text(evidenceSnapshotJson, 20_000);
    }
    public Guid AssessmentId { get; private set; }
    public Guid ControlPointId { get; private set; }
    public ComplianceEvaluationResult Result { get; private set; }
    public decimal ScorePercent { get; private set; }
    public string EvaluatorVersion { get; private set; } = string.Empty;
    public string EvaluatedBy { get; private set; } = string.Empty;
    public DateTimeOffset EvaluatedAt { get; private set; }
    public string? Notes { get; private set; }
    public string EvidenceSnapshotJson { get; private set; } = "[]";
}
