using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class DelegatedApprovalRequest : TenantEntity
{
    private DelegatedApprovalRequest() { }
    public DelegatedApprovalRequest(Guid id, Guid policyId, Guid organisationNodeId, Guid? farmId, string targetType, Guid targetId, string actionType, string requestJson, int requiredApprovals, DateTimeOffset requestedAt, DateTimeOffset expiresAt, string requestedBy) : base(id)
    { if (policyId == Guid.Empty || organisationNodeId == Guid.Empty || farmId == Guid.Empty || targetId == Guid.Empty || requiredApprovals is < 1 or > 20 || expiresAt <= requestedAt) throw new ArgumentException("Approval request scope, target, quorum, or validity is invalid."); PolicyId = policyId; OrganisationNodeId = organisationNodeId; FarmId = farmId; TargetType = PublicApiClient.Text(targetType, 120); TargetId = targetId; ActionType = PublicApiClient.Text(actionType, 120); RequestJson = PublicApiClient.Text(requestJson, 100_000); RequiredApprovals = requiredApprovals; RequestedAt = requestedAt.ToUniversalTime(); ExpiresAt = expiresAt.ToUniversalTime(); RequestedBy = PublicApiClient.Text(requestedBy, 240); Status = DelegatedApprovalStatus.Pending; }
    public Guid PolicyId { get; private set; } public Guid OrganisationNodeId { get; private set; } public Guid? FarmId { get; private set; } public string TargetType { get; private set; } = ""; public Guid TargetId { get; private set; } public string ActionType { get; private set; } = ""; public string RequestJson { get; private set; } = "{}"; public int RequiredApprovals { get; private set; } public int ApprovalCount { get; private set; } public DelegatedApprovalStatus Status { get; private set; } public DateTimeOffset RequestedAt { get; private set; } public DateTimeOffset ExpiresAt { get; private set; } public string RequestedBy { get; private set; } = ""; public DateTimeOffset? DecidedAt { get; private set; }
    public void RecordDecision(ApprovalDecisionKind decision, DateTimeOffset at) { if (Status != DelegatedApprovalStatus.Pending || at >= ExpiresAt) throw new InvalidOperationException("Approval request is not pending or has expired."); if (decision == ApprovalDecisionKind.Reject) { Status = DelegatedApprovalStatus.Rejected; DecidedAt = at.ToUniversalTime(); } else { ApprovalCount++; if (ApprovalCount >= RequiredApprovals) { Status = DelegatedApprovalStatus.Approved; DecidedAt = at.ToUniversalTime(); } } }
    public void Cancel(DateTimeOffset at) { if (Status != DelegatedApprovalStatus.Pending) throw new InvalidOperationException("Only a pending request can be cancelled."); Status = DelegatedApprovalStatus.Cancelled; DecidedAt = at.ToUniversalTime(); }
    public void Expire(DateTimeOffset at) { if (Status != DelegatedApprovalStatus.Pending || at < ExpiresAt) throw new InvalidOperationException("Request is not eligible to expire."); Status = DelegatedApprovalStatus.Expired; DecidedAt = at.ToUniversalTime(); }
}
