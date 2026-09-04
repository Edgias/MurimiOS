using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.DecideApproval;
using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using ApprovalDecision = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecision;
using ApprovalDecisionKind = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecisionKind;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class DecideApprovalHandler(IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant) : IRequestHandler<DecideApprovalCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(DecideApprovalCommand message, CancellationToken cancellationToken) => DecideApprovalAsync(message.requestId, message.request, db, actor, tenant, cancellationToken);

    private static async Task<UseCaseResult> DecideApprovalAsync(Guid requestId, ApprovalDecisionRequest request,
        IPlatformDbContext db, IRequestActorContext actor, ITenantContext tenant, CancellationToken ct)
    {
        if (actor.MembershipId is null) return UseCaseResults.Forbid(); DelegatedApprovalRequest? approval = await db.DelegatedApprovalRequests.SingleOrDefaultAsync(x => x.Id == requestId, ct); if (approval is null) return UseCaseResults.NotFound();
        ApprovalPolicy policy = await db.ApprovalPolicies.SingleAsync(x => x.Id == approval.PolicyId, ct); if (!actor.HasPermission(policy.ApproverPermission)) return UseCaseResults.Forbid();
        if (approval.RequestedBy == actor.Subject) return Conflict("self_approval_forbidden", "The requester cannot approve or reject their own request.");
        if (await db.ApprovalDecisions.AnyAsync(x => x.RequestId == requestId && x.MembershipId == actor.MembershipId, ct)) return Conflict("approval_already_decided", "This membership already decided the request.");
        try { DateTimeOffset now = DateTimeOffset.UtcNow; approval.RecordDecision(request.Decision, now); var decision = new ApprovalDecision(Guid.NewGuid(), requestId, actor.MembershipId.Value, actor.Subject, request.Decision, request.Reason, now); db.ApprovalDecisions.Add(decision); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"Enterprise.Approval{approval.Status}", JsonSerializer.Serialize(new { approval.Id, decision.Decision, approval.Status, approval.ApprovalCount }), now)); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(new { approval, decision }); }
        catch (InvalidOperationException e) { return Conflict("approval_decision_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
