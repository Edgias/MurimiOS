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
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateApprovalRequest;
using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using ApprovalPolicyStatus = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicyStatus;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateApprovalRequestHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant) : IRequestHandler<CreateApprovalRequestCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateApprovalRequestCommand message, CancellationToken cancellationToken) => CreateApprovalRequestAsync(message.request, db, scope, actor, tenant, cancellationToken);

    private static async Task<UseCaseResult> CreateApprovalRequestAsync(CreateDelegatedApprovalRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, CancellationToken ct)
    {
        ApprovalPolicy? policy = await db.ApprovalPolicies.SingleOrDefaultAsync(x => x.Id == request.PolicyId && x.Status == ApprovalPolicyStatus.Active, ct); if (policy is null) return UseCaseResults.NotFound();
        if (policy.ActionType != request.ActionType || !(await scope.ResolveAsync(ct)).AllowsNode(policy.OrganisationNodeId)) return Bad("approval_scope_invalid", "Policy action or organisation scope does not match.");
        if (request.FarmId.HasValue && !await scope.CanAccessFarmAsync(request.FarmId.Value, ct)) return UseCaseResults.NotFound();
        try { DateTimeOffset now = DateTimeOffset.UtcNow; var approval = new DelegatedApprovalRequest(Id(request.Id), policy.Id, policy.OrganisationNodeId, request.FarmId, request.TargetType, request.TargetId, request.ActionType, request.RequestJson, policy.RequiredApprovals, now, request.ExpiresAt, actor.Subject); db.DelegatedApprovalRequests.Add(approval); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "Enterprise.ApprovalRequested", JsonSerializer.Serialize(new { approval.Id, approval.TargetType, approval.TargetId, approval.ActionType, approval.RequiredApprovals, approval.ExpiresAt }), now)); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/enterprise/approval-requests/{approval.Id}", approval); }
        catch (ArgumentException e) { return Bad("approval_request_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
}
