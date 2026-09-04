using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ComplianceEvidenceLink : TenantEntity
{
    private ComplianceEvidenceLink() { }
    public ComplianceEvidenceLink(Guid id, Guid evaluationId, Guid evidenceAttachmentId, string linkedBy, DateTimeOffset linkedAt) : base(id)
    {
        if (evaluationId == Guid.Empty || evidenceAttachmentId == Guid.Empty) throw new ArgumentException("Evaluation and evidence are required.");
        EvaluationId = evaluationId; EvidenceAttachmentId = evidenceAttachmentId;
        LinkedBy = ComplianceStandardVersion.Text(linkedBy, 240); LinkedAt = linkedAt.ToUniversalTime();
    }
    public Guid EvaluationId { get; private set; }
    public Guid EvidenceAttachmentId { get; private set; }
    public string LinkedBy { get; private set; } = string.Empty;
    public DateTimeOffset LinkedAt { get; private set; }
}
