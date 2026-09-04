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
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.CreateGrant;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class CreateGrantHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, IOrganisationScopeAccess scope, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateGrantCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateGrantCommand message, CancellationToken cancellationToken) => CreateGrantAsync(message.request, db, tenant, actor, scope, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateGrantAsync(CreateDataGrantRequest request, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, IOrganisationScopeAccess scope, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        if (request.EntityCategories is null || request.Attributes is null || request.FarmIds is null
            || request.SeasonIds is null || request.Commodities is null)
            return Bad("grant_scope_required", "Categories, attributes, farms, seasons, and commodities must be supplied as arrays.");
        SupplyRelationship? relationship = await db.SupplyRelationships.SingleOrDefaultAsync(x => x.Id == request.RelationshipId
            && x.ProducerTenantId == tenant.TenantId && x.Status == SupplyRelationshipStatus.Active, cancellationToken);
        if (relationship is null) return UseCaseResults.NotFound();
        Guid[] farmIds = request.FarmIds.Distinct().ToArray();
        foreach (Guid farmId in farmIds)
            if (!await scope.CanAccessFarmAsync(farmId, cancellationToken) || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken))
                return Bad("farm_scope_invalid", "Every granted farm must exist within the caller's organisation scope.");
        if (request.SeasonIds.Count > 0 && await db.GrowingSeasons.CountAsync(x => request.SeasonIds.Contains(x.Id)
            && farmIds.Contains(x.FarmId), cancellationToken) != request.SeasonIds.Distinct().Count())
            return Bad("season_scope_invalid", "Every granted season must belong to a granted farm.");

        DataGrant? previous = null; int version = 1; Guid seriesId;
        if (request.PreviousGrantId is { } previousId)
        {
            previous = await db.DataGrants.SingleOrDefaultAsync(x => x.Id == previousId && x.RelationshipId == relationship.Id
                && x.ProducerTenantId == tenant.TenantId, cancellationToken);
            if (previous is null) return Bad("previous_grant_invalid", "The previous grant is unavailable in this relationship.");
            version = previous.GrantVersion + 1; seriesId = previous.GrantSeriesId;
        }
        else seriesId = Guid.NewGuid();

        DateTimeOffset now = clock.GetUtcNow(); Guid id = ValidId(request.Id);
        DataGrant grant;
        try
        {
            grant = new DataGrant(id, seriesId, relationship.Id, relationship.ProducerTenantId, relationship.BuyerTenantId,
                request.Name, request.Purpose, request.EntityCategories, request.Attributes, farmIds, request.SeasonIds,
                request.Commodities, request.ValidFrom, request.ValidTo, request.DownstreamDisclosureAllowed,
                version, previous?.Id, now);
        }
        catch (ArgumentException ex) { return Bad("data_grant_invalid", ex.Message); }
        if (previous is not null && previous.Status == DataGrantStatus.Active)
            previous.Revoke(tenant.TenantId, now, $"Superseded by grant {id} version {version}.");
        db.DataGrants.Add(grant);
        AddManagementAudit(db, tenant, actor, "Granted", nameof(DataGrant), id,
            new { relationship.Id, grant.RecipientTenantId, grant.Purpose, grant.EntityCategories, grant.FarmIds, grant.ValidFrom, grant.ValidTo },
            now, context.User.CorrelationId);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/supply-network/grants/{id}", ToResponse(grant));
    }


    private static void AddManagementAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string entityType, Guid entityId, object changes, DateTimeOffset at, string correlationId) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, at, actor.Subject, actor.MembershipId,
            action, entityType, entityId, null, actor.ScopeOrganisationNodeId, JsonSerializer.Serialize(changes),
            Activity.Current?.TraceId.ToString() ?? correlationId));

    private static Guid ValidId(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static SupplyRelationshipResponse ToResponse(SupplyRelationship x) => new(x.Id, x.ProducerTenantId,
        x.BuyerTenantId, x.RequestedByTenantId, x.RelationshipName, x.Status, x.RequestedAt, x.RespondedAt,
        x.RevokedAt, x.RevokedByTenantId, x.RevocationReason, x.Version);

    private static DataGrantResponse ToResponse(DataGrant x) => new(x.Id, x.GrantSeriesId, x.RelationshipId,
        x.ProducerTenantId, x.RecipientTenantId, x.Name, x.Purpose, x.EntityCategories, x.Attributes, x.FarmIds,
        x.SeasonIds, x.Commodities, x.ValidFrom, x.ValidTo, x.DownstreamDisclosureAllowed, x.GrantVersion,
        x.PreviousGrantId, x.Status, x.RevokedAt, x.RevocationReason);

}
