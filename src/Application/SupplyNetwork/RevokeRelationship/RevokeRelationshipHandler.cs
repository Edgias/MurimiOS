using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.RevokeRelationship;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class RevokeRelationshipHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<RevokeRelationshipCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RevokeRelationshipCommand message, CancellationToken cancellationToken) => RevokeRelationshipAsync(message.relationshipId, message.request, db, tenant, actor, clock, context, cancellationToken);
    private static Task<UseCaseResult> RevokeRelationshipAsync(Guid relationshipId, RevokeSupplyAccessRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken) =>        RespondRelationshipAsync(relationshipId, "revoke", request.Reason, db, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> RespondRelationshipAsync(Guid id, string action, string? reason,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        SupplyRelationship? relationship = await db.SupplyRelationships.SingleOrDefaultAsync(x => x.Id == id
            && (x.ProducerTenantId == tenant.TenantId || x.BuyerTenantId == tenant.TenantId), cancellationToken);
        if (relationship is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try
        {
            if (action == "accept") relationship.Accept(tenant.TenantId, now);
            else if (action == "reject") relationship.Reject(tenant.TenantId, now);
            else relationship.Revoke(tenant.TenantId, now, reason ?? string.Empty);
        }
        catch (InvalidOperationException ex) { return UseCaseResults.Conflict(new { code = "invalid_relationship_transition", message = ex.Message }); }
        catch (ArgumentException ex) { return Bad("relationship_invalid", ex.Message); }
        AddManagementAudit(db, tenant, actor, action == "accept" ? "Accepted" : action == "reject" ? "Rejected" : "Revoked",
            nameof(SupplyRelationship), id, new { reason }, now, context.User.CorrelationId);
        await db.SaveChangesAsync(cancellationToken); return UseCaseResults.Ok(ToResponse(relationship));
    }


    private static void AddManagementAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string entityType, Guid entityId, object changes, DateTimeOffset at, string correlationId) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, at, actor.Subject, actor.MembershipId,
            action, entityType, entityId, null, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(changes),
            Activity.Current?.TraceId.ToString() ?? correlationId));

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static SupplyRelationshipResponse ToResponse(SupplyRelationship x) => new(x.Id, x.ProducerTenantId,
        x.BuyerTenantId, x.RequestedByTenantId, x.RelationshipName, x.Status, x.RequestedAt, x.RespondedAt,
        x.RevokedAt, x.RevokedByTenantId, x.RevocationReason, x.Version);

    private static DataGrantResponse ToResponse(DataGrant x) => new(x.Id, x.GrantSeriesId, x.RelationshipId,
        x.ProducerTenantId, x.RecipientTenantId, x.Name, x.Purpose, x.EntityCategories, x.Attributes, x.FarmIds,
        x.SeasonIds, x.Commodities, x.ValidFrom, x.ValidTo, x.DownstreamDisclosureAllowed, x.GrantVersion,
        x.PreviousGrantId, x.Status, x.RevokedAt, x.RevocationReason);

}
