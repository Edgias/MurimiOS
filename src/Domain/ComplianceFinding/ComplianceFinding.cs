using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceFinding;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ComplianceFinding : TenantEntity
{
    private ComplianceFinding() { }
    public ComplianceFinding(Guid id, Guid assessmentId, Guid controlPointId, ComplianceFindingSeverity severity,
        string title, string description, DateOnly dueOn, DateTimeOffset openedAt) : base(id)
    {
        if (assessmentId == Guid.Empty || controlPointId == Guid.Empty) throw new ArgumentException("Assessment and control are required.");
        if (!Enum.IsDefined(severity) || dueOn < DateOnly.FromDateTime(openedAt.UtcDateTime)) throw new ArgumentOutOfRangeException(nameof(dueOn));
        AssessmentId = assessmentId; ControlPointId = controlPointId; Severity = severity;
        Title = ComplianceStandardVersion.Text(title, 240); Description = ComplianceStandardVersion.Text(description, 4_000);
        DueOn = dueOn; OpenedAt = openedAt.ToUniversalTime(); Status = ComplianceFindingStatus.Open;
    }
    public Guid AssessmentId { get; private set; }
    public Guid ControlPointId { get; private set; }
    public ComplianceFindingSeverity Severity { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateOnly DueOn { get; private set; }
    public DateTimeOffset OpenedAt { get; private set; }
    public ComplianceFindingStatus Status { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }
    public string? WaiverReason { get; private set; }
    public void MarkActionPending() { if (Status != ComplianceFindingStatus.Open) throw new InvalidOperationException("Only an open finding can receive an action."); Status = ComplianceFindingStatus.ActionPending; }
    public void MarkVerificationPending() { if (Status is not (ComplianceFindingStatus.ActionPending or ComplianceFindingStatus.Open)) throw new InvalidOperationException("The finding cannot enter verification."); Status = ComplianceFindingStatus.VerificationPending; }
    public void Close(DateTimeOffset at, string actor) { if (Status != ComplianceFindingStatus.VerificationPending) throw new InvalidOperationException("Only a verified finding can close."); Status = ComplianceFindingStatus.Closed; ClosedAt = at.ToUniversalTime(); ClosedBy = ComplianceStandardVersion.Text(actor, 240); }
    public void Reopen() { if (Status != ComplianceFindingStatus.VerificationPending) throw new InvalidOperationException("Only a finding awaiting verification can be reopened."); Status = ComplianceFindingStatus.ActionPending; }
    public void Waive(DateTimeOffset at, string actor, string reason) { if (Status is ComplianceFindingStatus.Closed or ComplianceFindingStatus.Waived) throw new InvalidOperationException("The finding is already closed."); Status = ComplianceFindingStatus.Waived; ClosedAt = at.ToUniversalTime(); ClosedBy = ComplianceStandardVersion.Text(actor, 240); WaiverReason = ComplianceStandardVersion.Text(reason, 2_000); }
}
