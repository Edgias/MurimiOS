using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.CreateApprovalPolicy;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;

public sealed class CreateApprovalPolicyHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, IRequestActorContext actor) : IRequestHandler<CreateApprovalPolicyCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateApprovalPolicyCommand message, CancellationToken cancellationToken) => CreateApprovalPolicyAsync(message.request, db, scope, actor, cancellationToken);

    private static async Task<UseCaseResult> CreateApprovalPolicyAsync(CreateApprovalPolicyRequest request, IPlatformDbContext db,
        IOrganisationScopeAccess scope, IRequestActorContext actor, CancellationToken ct)
    {
        if (!(await scope.ResolveAsync(ct)).AllowsNode(request.OrganisationNodeId)) return UseCaseResults.NotFound();
        if (!PlatformPermissions.All.Contains(request.ApproverPermission)) return Bad("approver_permission_invalid", "Approver permission is not registered.");
        try { var policy = new ApprovalPolicy(Id(request.Id), request.Code, request.Name, request.OrganisationNodeId, request.ActionType, request.ApproverPermission, request.RequiredApprovals, DateTimeOffset.UtcNow, actor.Subject); db.ApprovalPolicies.Add(policy); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/enterprise/approval-policies/{policy.Id}", policy); }
        catch (ArgumentException e) { return Bad("approval_policy_invalid", e.Message); }
    }

    private static void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message }); 
}
