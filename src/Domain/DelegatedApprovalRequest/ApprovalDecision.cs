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

public sealed class ApprovalDecision : TenantEntity
{
    private ApprovalDecision() { }
    public ApprovalDecision(Guid id, Guid requestId, Guid membershipId, string actorSubject, ApprovalDecisionKind decision, string? reason, DateTimeOffset decidedAt) : base(id)
    { if (requestId == Guid.Empty || membershipId == Guid.Empty) throw new ArgumentException("Request and membership are required."); RequestId = requestId; MembershipId = membershipId; ActorSubject = PublicApiClient.Text(actorSubject, 240); Decision = decision; Reason = string.IsNullOrWhiteSpace(reason) ? null : PublicApiClient.Text(reason, 2000); DecidedAt = decidedAt.ToUniversalTime(); }
    public Guid RequestId { get; private set; } public Guid MembershipId { get; private set; } public string ActorSubject { get; private set; } = ""; public ApprovalDecisionKind Decision { get; private set; } public string? Reason { get; private set; } public DateTimeOffset DecidedAt { get; private set; }
}
