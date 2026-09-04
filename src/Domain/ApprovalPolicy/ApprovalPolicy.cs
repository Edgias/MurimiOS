using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

namespace EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

public sealed class ApprovalPolicy : TenantEntity
{
    private ApprovalPolicy() { }
    public ApprovalPolicy(Guid id, string code, string name, Guid organisationNodeId, string actionType, string approverPermission, int requiredApprovals, DateTimeOffset createdAt, string createdBy) : base(id)
    { if (organisationNodeId == Guid.Empty || requiredApprovals is < 1 or > 20) throw new ArgumentException("Organisation scope and 1-20 approvals are required."); Code = PublicApiClient.Text(code, 80).ToLowerInvariant(); Name = PublicApiClient.Text(name, 200); OrganisationNodeId = organisationNodeId; ActionType = PublicApiClient.Text(actionType, 120); ApproverPermission = PublicApiClient.Text(approverPermission, 120); RequiredApprovals = requiredApprovals; AuthoredAt = createdAt.ToUniversalTime(); AuthoredBy = PublicApiClient.Text(createdBy, 240); Status = ApprovalPolicyStatus.Active; }
    public string Code { get; private set; } = ""; public string Name { get; private set; } = ""; public Guid OrganisationNodeId { get; private set; } public string ActionType { get; private set; } = ""; public string ApproverPermission { get; private set; } = ""; public int RequiredApprovals { get; private set; } public ApprovalPolicyStatus Status { get; private set; } public DateTimeOffset AuthoredAt { get; private set; } public string AuthoredBy { get; private set; } = ""; public DateTimeOffset? RetiredAt { get; private set; }
    public void Retire(DateTimeOffset at) { if (Status != ApprovalPolicyStatus.Active) throw new InvalidOperationException("Policy is already retired."); Status = ApprovalPolicyStatus.Retired; RetiredAt = at.ToUniversalTime(); }
}
