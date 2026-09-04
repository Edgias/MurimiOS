using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Access.Update;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using MembershipPermissionGrant = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipPermissionGrant;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;


public sealed class UpdateHandler(IPlatformDbContext db, IRequestActorContext actor, IOrganisationScopeAccess scopeAccess, ITenantContext tenant, TimeProvider timeProvider) : IRequestHandler<UpdateCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(UpdateCommand message, CancellationToken cancellationToken) => UpdateAsync(message.membershipId, message.request, db, actor, scopeAccess, tenant, timeProvider, cancellationToken);

    private static async Task<UseCaseResult>UpdateAsync(
        Guid membershipId,
        UpdateMembershipAccessRequest request,
        IPlatformDbContext db,
        IRequestActorContext actor,
        IOrganisationScopeAccess scopeAccess,
        ITenantContext tenant,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (!TryValidatePermissions(request.PermissionGrants, out string[] permissions, out string? permissionError))
            return UseCaseResults.BadRequest(new { code = "invalid_permission", message = permissionError });
        ResolvedOrganisationScope actorScope = await scopeAccess.ResolveAsync(cancellationToken);
        if (!ScopeAssignmentAllowed(actorScope, request.ScopeOrganisationNodeId))
            return UseCaseResults.Json(new { code = "scope_escalation", message = "The requested scope exceeds the actor's organisation scope." }, statusCode: 403);
        TenantMembership? membership = await db.TenantMemberships.SingleOrDefaultAsync(x => x.Id == membershipId, cancellationToken);
        if (membership is null) return UseCaseResults.NotFound();
        if (!actorScope.IsUnrestricted
            && (membership.ScopeOrganisationNodeId is not { } existingScope || !actorScope.AllowsNode(existingScope)))
            return UseCaseResults.NotFound();
        if (request.ScopeOrganisationNodeId is { } scopeId
            && !await db.OrganisationNodes.AnyAsync(x => x.Id == scopeId, cancellationToken))
            return UseCaseResults.BadRequest(new { code = "scope_not_found", message = "The organisation scope is unavailable in this tenant." });

        bool removesOwner = membership.Role == TenantRole.Owner
            && (request.Role != TenantRole.Owner || request.Status != MembershipStatus.Active);
        if (removesOwner)
        {
            bool anotherOwnerExists = await db.TenantMemberships.AnyAsync(
                x => x.Id != membershipId && x.Role == TenantRole.Owner && x.Status == MembershipStatus.Active,
                cancellationToken);
            if (!anotherOwnerExists)
                return UseCaseResults.Conflict(new { code = "last_owner", message = "The tenant must retain at least one active owner." });
        }

        if (actor.MembershipId == membershipId && request.Status != MembershipStatus.Active)
            return UseCaseResults.Conflict(new { code = "self_suspension", message = "A member cannot suspend or revoke their own active session." });

        membership.UpdateAccess(request.Role, request.Status, request.ScopeOrganisationNodeId);
        MembershipPermissionGrant[] existing = await db.MembershipPermissionGrants
            .Where(x => x.MembershipId == membershipId)
            .ToArrayAsync(cancellationToken);
        db.MembershipPermissionGrants.RemoveRange(existing.Where(x => !permissions.Contains(x.Permission, StringComparer.Ordinal)));
        foreach (string permission in permissions.Except(existing.Select(x => x.Permission), StringComparer.Ordinal))
            db.MembershipPermissionGrants.Add(new MembershipPermissionGrant(Guid.NewGuid(), membership.Id, permission));
        AddOutbox(db, tenant.TenantId, "Access.MembershipUpdated", membership, permissions, timeProvider.GetUtcNow());
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(ToResponse(membership, permissions));
    }


    private static bool TryValidatePermissions(IEnumerable<string>? requested, out string[] permissions, out string? error)
    {
        permissions = (requested ?? [])
            .Select(x => x.Trim().ToLowerInvariant())
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        string[] unknown = permissions.Where(x => !PlatformPermissions.All.Contains(x)).ToArray();
        error = unknown.Length == 0 ? null : $"Unknown permissions: {string.Join(", ", unknown)}";
        return unknown.Length == 0;
    }


    private static bool ScopeAssignmentAllowed(ResolvedOrganisationScope actorScope, Guid? requestedScope) =>
        actorScope.IsUnrestricted
        || requestedScope is { } scopeId && actorScope.AllowsNode(scopeId);


    private static MembershipResponse ToResponse(TenantMembership membership, IEnumerable<string> grants)
    {
        string[] explicitGrants = grants.Order(StringComparer.Ordinal).ToArray();
        string[] effective = PlatformPermissions.ForRole(membership.Role)
            .Concat(explicitGrants)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
        return new MembershipResponse(
            membership.Id, membership.ExternalSubject, membership.DisplayName, membership.Email,
            membership.Role, membership.Status, membership.ScopeOrganisationNodeId, explicitGrants, effective);
    }


    private static void AddOutbox(
        IPlatformDbContext db,
        Guid tenantId,
        string type,
        TenantMembership membership,
        IReadOnlyCollection<string> grants,
        DateTimeOffset occurredAt)
    {
        string payload = JsonSerializer.Serialize(new
        {
            membership.Id, membership.ExternalSubject, membership.Role, membership.Status,
            membership.ScopeOrganisationNodeId, PermissionGrants = grants
        });
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenantId, type, payload, occurredAt));
    }

}
