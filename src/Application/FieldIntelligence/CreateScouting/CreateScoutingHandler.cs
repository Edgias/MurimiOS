using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.CreateScouting;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
using MapLayerKind = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.MapLayerKind;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;

public sealed class CreateScoutingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateScoutingCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateScoutingCommand message, CancellationToken cancellationToken) => CreateScoutingAsync(message.fieldId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateScoutingAsync(Guid fieldId, CreateScoutingObservationRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        var target = await LoadFieldAsync(fieldId, db, scope, cancellationToken);
        if (target.Field is null) return UseCaseResults.NotFound();
        Guid boundaryId = request.BoundaryVersionId ?? target.Boundary!.Id;
        FieldBoundaryVersion? boundary = boundaryId == target.Boundary!.Id ? target.Boundary
            : await db.FieldBoundaryVersions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == boundaryId && x.FieldId == fieldId, cancellationToken);
        if (boundary is null) return Bad("scouting_boundary_invalid", "The boundary snapshot must belong to this field.");
        if (request.CropZoneId is { } zoneId && !await db.CropZones.AnyAsync(x => x.Id == zoneId && x.FieldId == fieldId, cancellationToken))
            return Bad("scouting_crop_zone_invalid", "The crop zone must belong to this field.");
        if (request.EvidenceId is { } evidenceId && !await db.EvidenceAttachments.AnyAsync(x => x.Id == evidenceId
            && db.CropZones.Any(zone => zone.Id == x.CropZoneId && zone.FieldId == fieldId), cancellationToken))
            return Bad("scouting_evidence_invalid", "The evidence must belong to a crop zone on this field.");
        try
        {
            DateTimeOffset now = clock.GetUtcNow(); Point? point = request.Longitude is null && request.Latitude is null ? null
                : request.Longitude is not null && request.Latitude is not null ? new Point((double)request.Longitude.Value, (double)request.Latitude.Value) { SRID = 4326 }
                : throw new ArgumentException("Latitude and longitude must be supplied together.");
            Guid id = NonEmptyOrNew(request.Id);
            var row = new ScoutingObservation(id, target.Field.FarmId, fieldId, boundary.Id, request.CropZoneId,
                request.Category, request.Severity, request.IssueCode, request.Title, request.Notes,
                request.ObservedAt, now, actor.Subject, point, request.EvidenceId, request.SourceCommandId);
            db.ScoutingObservations.Add(row);
            db.FieldMapLayers.Add(BuildScoutingLayer(row, target.Field.Name, boundary, now));
            AddEvent(db, tenant, actor, nameof(ScoutingObservation), id, target.Field.FarmId,
                "ScoutingRecorded", "FieldIntelligence.ScoutingRecorded", new { row.Id, row.FieldId, row.BoundaryVersionId, row.Category, row.Severity, row.IssueCode, row.ObservedAt }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/field-intelligence/fields/{fieldId}/scouting/{id}", ToScouting(row));
        }
        catch (ArgumentException ex) { return Bad("scouting_observation_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("scouting_observation_conflict", DbMessage(ex)); }
    }


    private static async Task<FieldTarget> LoadFieldAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        FarmField? field = await db.FarmFields.AsNoTracking().SingleOrDefaultAsync(x => x.Id == fieldId, cancellationToken);
        if (field is null || !await scope.CanAccessFarmAsync(field.FarmId, cancellationToken)) return new(null, null);
        FieldBoundaryVersion? boundary = await db.FieldBoundaryVersions.AsNoTracking().Where(x => x.FieldId == fieldId && x.ValidTo == null)
            .OrderByDescending(x => x.ValidFrom).FirstOrDefaultAsync(cancellationToken);
        return boundary is null ? new(null, null) : new(field, boundary);
    }


    private static FieldMapLayer BuildScoutingLayer(ScoutingObservation row, string fieldName,
        FieldBoundaryVersion boundary, DateTimeOffset now)
    {
        object geometry = row.Location is null ? MultiPolygonCoordinates(boundary.Boundary) : new { type = "Point", coordinates = new[] { row.Location.X, row.Location.Y } };
        string geoJson = JsonSerializer.Serialize(new { type = "Feature", geometry, properties = new { row.Id, row.FieldId, row.Category, row.Severity, row.IssueCode, row.Title, row.ObservedAt } });
        string color = row.Severity >= InsightSeverity.Critical ? "#991b1b" : row.Severity >= InsightSeverity.Warning ? "#dc2626" : row.Severity >= InsightSeverity.Watch ? "#f59e0b" : "#2563eb";
        string style = JsonSerializer.Serialize(new { markerColor = color, markerSize = 10, fillColor = color, fillOpacity = 0.2 });
        return new FieldMapLayer(Guid.NewGuid(), row.FarmId, row.FieldId, row.BoundaryVersionId,
            MapLayerKind.Scouting, $"{fieldName} scouting {row.ObservedAt:yyyy-MM-dd HH:mm}",
            nameof(ScoutingObservation), row.Id, row.ObservedAt, now, row.ObservedAt, null,
            geoJson, style, InsightQuality.Valid);
    }


    private static object MultiPolygonCoordinates(MultiPolygon boundary) => new
    {
        type = "MultiPolygon",
        coordinates = Enumerable.Range(0, boundary.NumGeometries).Select(polygonIndex =>
        {
            var polygon = (Polygon)boundary.GetGeometryN(polygonIndex);
            var rings = new List<double[][]> { polygon.ExteriorRing.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray() };
            rings.AddRange(Enumerable.Range(0, polygon.NumInteriorRings).Select(index => polygon.GetInteriorRingN(index).Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()));
            return rings.ToArray();
        }).ToArray()
    };

    private static ScoutingObservationResponse ToScouting(ScoutingObservation x) => new(x.Id, x.FarmId, x.FieldId,
        x.BoundaryVersionId, x.CropZoneId, x.Category, x.Severity, x.IssueCode, x.Title, x.Notes,
        x.ObservedAt, x.RecordedAt, x.RecordedBy, x.Location?.Y, x.Location?.X, x.EvidenceId, x.SourceCommandId);


    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string entityType, Guid entityId, Guid? farmId, string action, string eventType, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        string syncOperation = action switch
        {
            "InsightAlertAcknowledged" => "AlertAcknowledged",
            "InsightAlertDismissed" => "AlertDismissed",
            "AlertWorkOrderCreated" => "AlertWorkCreated",
            _ => action
        };
        if (farmId is { } farm) db.SyncChanges.Add(new SyncChange(tenant.TenantId, farm, entityType, entityId, syncOperation, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static Guid NonEmptyOrNew(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static string DbMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private sealed record FieldTarget(FarmField? Field, FieldBoundaryVersion? Boundary);

}
