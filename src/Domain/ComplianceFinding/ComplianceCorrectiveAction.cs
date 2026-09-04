using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceFinding;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ComplianceCorrectiveAction : TenantEntity
{
    private ComplianceCorrectiveAction() { }
    public ComplianceCorrectiveAction(Guid id, Guid findingId, string action, string owner, DateOnly dueOn) : base(id)
    {
        if (findingId == Guid.Empty) throw new ArgumentException("A finding is required.");
        FindingId = findingId; Action = ComplianceStandardVersion.Text(action, 4_000);
        Owner = ComplianceStandardVersion.Text(owner, 240); DueOn = dueOn; Status = CorrectiveActionStatus.Planned;
    }
    public Guid FindingId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Owner { get; private set; } = string.Empty;
    public DateOnly DueOn { get; private set; }
    public CorrectiveActionStatus Status { get; private set; }
    public string? CompletionNotes { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public void Start() { if (Status is not (CorrectiveActionStatus.Planned or CorrectiveActionStatus.Rejected)) throw new InvalidOperationException("Only a planned or rejected action can start."); Status = CorrectiveActionStatus.InProgress; RejectionReason = null; }
    public void Submit(DateTimeOffset at, string notes) { if (Status != CorrectiveActionStatus.InProgress) throw new InvalidOperationException("Only an in-progress action can be submitted."); Status = CorrectiveActionStatus.Submitted; SubmittedAt = at.ToUniversalTime(); CompletionNotes = ComplianceStandardVersion.Text(notes, 4_000); }
    public void Verify(DateTimeOffset at, string actor) { if (Status != CorrectiveActionStatus.Submitted) throw new InvalidOperationException("Only a submitted action can be verified."); Status = CorrectiveActionStatus.Verified; VerifiedAt = at.ToUniversalTime(); VerifiedBy = ComplianceStandardVersion.Text(actor, 240); }
    public void Reject(string reason) { if (Status != CorrectiveActionStatus.Submitted) throw new InvalidOperationException("Only a submitted action can be rejected."); Status = CorrectiveActionStatus.Rejected; RejectionReason = ComplianceStandardVersion.Text(reason, 2_000); }
    public void Cancel() { if (Status is CorrectiveActionStatus.Verified or CorrectiveActionStatus.Cancelled) throw new InvalidOperationException("The action is already closed."); Status = CorrectiveActionStatus.Cancelled; }
}
