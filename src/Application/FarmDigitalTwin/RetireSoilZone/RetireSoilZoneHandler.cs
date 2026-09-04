using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.RetireSoilZone;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class RetireSoilZoneHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<RetireSoilZoneCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RetireSoilZoneCommand message, CancellationToken cancellationToken) => RetireSoilZoneAsync(message.farmId, message.zoneId, db, scope, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> RetireSoilZoneAsync(Guid farmId, Guid zoneId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context,
        CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        SoilZone? zone = await db.SoilZones.SingleOrDefaultAsync(x => x.Id == zoneId && x.FarmId == farmId, ct);
        if (zone is null) return UseCaseResults.NotFound();
        try { zone.Retire(DateTimeOffset.UtcNow, actor.Subject); AddRecords(db, tenant, actor, "Retired", zone, zone.Id, farmId, "FarmDigitalTwin.SoilZoneRetired", context); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(zone); }
        catch (InvalidOperationException e) { return Conflict("soil_zone_transition_invalid", e.Message); }
    }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, object entity, Guid entityId, Guid farmId, string eventType, UseCaseContext context)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        object payload = entity switch
        {
            SiteInfrastructureAsset asset => Asset(asset),
            SoilZoneBoundaryVersion boundary => Boundary(boundary),
            SoilSample sample => Sample(sample, []),
            _ => entity
        };
        string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entity.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
        db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, entity.GetType().Name, entityId, action, json, now));
    }

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });


    private static object Asset(SiteInfrastructureAsset x) => new
    {
        x.Id, x.FarmId, x.FieldId, x.Code, x.Name, x.Type, x.Status,
        location = x.Location is null ? null : new { longitude = x.Location.X, latitude = x.Location.Y },
        x.InstalledOn, x.Capacity, x.CapacityUnit, details = ParseJson(x.DetailsJson),
        x.DecommissionedOn, x.DecommissionedAt, x.DecommissionedBy, x.DecommissionReason
    };

    private static object Boundary(SoilZoneBoundaryVersion boundary) => new
    {
        boundary.Id, boundary.SoilZoneId, boundary.FieldBoundaryVersionId, boundary.AreaHa,
        boundary.ValidFrom, boundary.ValidTo, boundary.Source,
        geometry = new { type = "MultiPolygon", coordinates = Coordinates(boundary.Boundary) }
    };

    private static object Sample(SoilSample sample, IEnumerable<SoilSampleResult> results) => new
    {
        sample.Id, sample.FarmId, sample.FieldId, sample.FieldBoundaryVersionId, sample.SoilZoneBoundaryVersionId,
        sample.SampleCode, location = new { longitude = sample.Location.X, latitude = sample.Location.Y },
        sample.SampledAt, sample.SampledBy, sample.DepthFromCm, sample.DepthToCm, sample.LaboratoryName,
        sample.LaboratoryReference, sample.Notes, sample.Status, sample.SubmittedAt, sample.AnalysedAt,
        sample.RejectedAt, sample.RejectionReason, results
    };

    private static JsonElement ParseJson(string json) { using JsonDocument document = JsonDocument.Parse(json); return document.RootElement.Clone(); }

    private static object Coordinates(MultiPolygon geometry) => geometry.Geometries.Cast<Polygon>().Select(polygon =>
        new[] { polygon.ExteriorRing.Coordinates.Select(x => new[] { x.X, x.Y }).ToArray() }).ToArray();

}
