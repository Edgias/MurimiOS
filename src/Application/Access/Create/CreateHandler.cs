using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Access.Create;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using MembershipPermissionGrant = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipPermissionGrant;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;


public sealed class CreateHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, ITenantContext tenant, TimeProvider timeProvider) : IRequestHandler<CreateCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCommand message, CancellationToken cancellationToken) => CreateAsync(message.request, db, scopeAccess, tenant, timeProvider, cancellationToken);

    private static async Task<UseCaseResult>CreateAsync(
        CreateMembershipRequest request,
        IPlatformDbContext db,
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
        if (request.ScopeOrganisationNodeId is { } scopeId
            && !await db.OrganisationNodes.AnyAsync(x => x.Id == scopeId, cancellationToken))
            return UseCaseResults.BadRequest(new { code = "scope_not_found", message = "The organisation scope is unavailable in this tenant." });

        if (await db.TenantMemberships.AnyAsync(x => x.ExternalSubject == request.ExternalSubject, cancellationToken))
            return UseCaseResults.Conflict(new { code = "membership_exists", message = "This identity already has a membership in the tenant." });

        var membership = new TenantMembership(
            Guid.NewGuid(), request.ExternalSubject, request.DisplayName, request.Email,
            request.Role, request.Status, request.ScopeOrganisationNodeId);
        db.TenantMemberships.Add(membership);
        foreach (string permission in permissions)
            db.MembershipPermissionGrants.Add(new MembershipPermissionGrant(Guid.NewGuid(), membership.Id, permission));
        AddOutbox(db, tenant.TenantId, "Access.MembershipCreated", membership, permissions, timeProvider.GetUtcNow());
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/access/memberships/{membership.Id}", ToResponse(membership, permissions));
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
