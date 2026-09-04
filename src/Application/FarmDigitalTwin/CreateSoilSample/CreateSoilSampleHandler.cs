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
namespace EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateSoilSample;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateSoilSampleHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<CreateSoilSampleCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateSoilSampleCommand message, CancellationToken cancellationToken) => CreateSoilSampleAsync(message.farmId, message.request, db, scope, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> CreateSoilSampleAsync(Guid farmId, CreateSoilSampleRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        UseCaseContext context, CancellationToken ct)
    {
        if (!await FarmAndFieldAsync(farmId, request.FieldId, db, scope, ct)) return UseCaseResults.NotFound();
        if (!await db.FieldBoundaryVersions.AnyAsync(x => x.Id == request.FieldBoundaryVersionId && x.FieldId == request.FieldId, ct))
            return Bad("field_boundary_invalid", "The field-boundary version does not belong to this field.");
        if (request.SoilZoneBoundaryVersionId.HasValue && !await (
            from boundary in db.SoilZoneBoundaryVersions join zone in db.SoilZones on boundary.SoilZoneId equals zone.Id
            where boundary.Id == request.SoilZoneBoundaryVersionId && zone.FieldId == request.FieldId select boundary.Id).AnyAsync(ct))
            return Bad("soil_zone_boundary_invalid", "The soil-zone boundary does not belong to this field.");
        try
        {
            var sample = new SoilSample(Id(request.Id), farmId, request.FieldId, request.FieldBoundaryVersionId,
                request.SoilZoneBoundaryVersionId, request.SampleCode,
                new Point(request.Longitude, request.Latitude) { SRID = 4326 }, request.SampledAt,
                request.SampledBy ?? actor.Subject, request.DepthFromCm, request.DepthToCm,
                request.LaboratoryName, request.LaboratoryReference, request.Notes);
            db.SoilSamples.Add(sample); AddRecords(db, tenant, actor, "Collected", sample, sample.Id, farmId,
                "FarmDigitalTwin.SoilSampleCollected", context); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/farms/{farmId}/digital-twin/soil-samples/{sample.Id}", Sample(sample, []));
        }
        catch (ArgumentException e) { return Bad("soil_sample_invalid", e.Message); }
        catch (DbUpdateException) { return Conflict("soil_sample_spatial_constraint", "The sample point must be inside its exact field and soil-zone boundary versions."); }
    }


    private static async Task<bool> FarmAndFieldAsync(Guid farmId, Guid? fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct) || !await db.Farms.AnyAsync(x => x.Id == farmId, ct)) return false;
        return !fieldId.HasValue || await db.FarmFields.AnyAsync(x => x.Id == fieldId && x.FarmId == farmId, ct);
    }

    private static object Sample(SoilSample sample, IEnumerable<SoilSampleResult> results) => new
    {
        sample.Id, sample.FarmId, sample.FieldId, sample.FieldBoundaryVersionId, sample.SoilZoneBoundaryVersionId,
        sample.SampleCode, location = new { longitude = sample.Location.X, latitude = sample.Location.Y },
        sample.SampledAt, sample.SampledBy, sample.DepthFromCm, sample.DepthToCm, sample.LaboratoryName,
        sample.LaboratoryReference, sample.Notes, sample.Status, sample.SubmittedAt, sample.AnalysedAt,
        sample.RejectedAt, sample.RejectionReason, results
    };

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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

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

    private static JsonElement ParseJson(string json) { using JsonDocument document = JsonDocument.Parse(json); return document.RootElement.Clone(); }

    private static object Coordinates(MultiPolygon geometry) => geometry.Geometries.Cast<Polygon>().Select(polygon =>
        new[] { polygon.ExteriorRing.Coordinates.Select(x => new[] { x.X, x.Y }).ToArray() }).ToArray();

}
