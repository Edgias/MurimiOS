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
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.CreateRelationship;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using DataGrantStatus = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrantStatus;
using SupplyPartyRole = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyPartyRole;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class CreateRelationshipHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext httpContext) : IRequestHandler<CreateRelationshipCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRelationshipCommand message, CancellationToken cancellationToken) => CreateRelationshipAsync(message.request, db, tenant, actor, clock, httpContext, cancellationToken);

    private static async Task<UseCaseResult> CreateRelationshipAsync(CreateSupplyRelationshipRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext httpContext, CancellationToken cancellationToken)
    {
        if (request.CounterpartyTenantId == Guid.Empty || request.CounterpartyTenantId == tenant.TenantId)
            return Bad("invalid_counterparty", "A distinct counterparty tenant is required.");
        if (!Enum.IsDefined(request.OurRole))
            return Bad("invalid_party_role", "OurRole must be Producer or Buyer.");
        Guid producerId = request.OurRole == SupplyPartyRole.Producer ? tenant.TenantId : request.CounterpartyTenantId;
        Guid buyerId = request.OurRole == SupplyPartyRole.Buyer ? tenant.TenantId : request.CounterpartyTenantId;
        if (await db.SupplyRelationships.AnyAsync(x => x.ProducerTenantId == producerId && x.BuyerTenantId == buyerId
            && (x.Status == SupplyRelationshipStatus.Pending || x.Status == SupplyRelationshipStatus.Active), cancellationToken))
            return UseCaseResults.Conflict(new { code = "relationship_exists" });
        DateTimeOffset now = clock.GetUtcNow(); Guid id = ValidId(request.Id);
        SupplyRelationship relationship;
        try { relationship = new SupplyRelationship(id, producerId, buyerId, tenant.TenantId, request.Name, now); }
        catch (ArgumentException ex) { return Bad("relationship_invalid", ex.Message); }
        db.SupplyRelationships.Add(relationship);
        AddManagementAudit(db, tenant, actor, "Requested", nameof(SupplyRelationship), id,
            new { producerId, buyerId, request.Name }, now, httpContext.User.CorrelationId);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Created($"/api/v1/supply-network/relationships/{id}", ToResponse(relationship));
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
