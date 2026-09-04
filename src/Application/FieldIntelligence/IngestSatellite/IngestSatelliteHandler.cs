using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.IngestSatellite;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
using MapLayerKind = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.MapLayerKind;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;

public sealed class IngestSatelliteHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<IngestSatelliteCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(IngestSatelliteCommand message, CancellationToken cancellationToken) => IngestSatelliteAsync(message.recordId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> IngestSatelliteAsync(Guid recordId, IngestSatelliteStatisticsRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        var source = await LoadIngestionSourceAsync(recordId, IntegrationFeedType.Satellite, db, scope, cancellationToken);
        if (source.Error is not null) return source.Error;
        if (request.Statistics is null || request.Statistics.Count is 0 or > 20)
            return Bad("satellite_batch_invalid", "A satellite batch must contain between 1 and 20 index statistics.");
        FarmField? field = await db.FarmFields.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.FieldId && x.FarmId == source.Record!.FarmId, cancellationToken);
        FieldBoundaryVersion? boundary = await db.FieldBoundaryVersions.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.BoundaryVersionId && x.FieldId == request.FieldId, cancellationToken);
        if (field is null || boundary is null) return Bad("satellite_field_invalid", "The field and boundary snapshot must belong to the ingestion farm.");
        if (!FeedAllowsField(source.Feed!, request.FieldId)) return Bad("satellite_feed_target_invalid", "This feed is assigned to a different field.");
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            var rows = request.Statistics.Select(item => new SatelliteFieldStatistic(NonEmptyOrNew(item.Id),
                source.Record!.FarmId, request.FieldId, request.BoundaryVersionId, source.Record.FeedId, source.Record.Id,
                item.ProviderAcquisitionId, item.Index, item.CustomIndex, item.AcquiredAt, now, item.Mean,
                item.Minimum, item.Maximum, item.StandardDeviation, item.ValidPixelPercent, item.CloudPercent,
                item.Quality)).ToArray();
            db.SatelliteFieldStatistics.AddRange(rows);
            foreach (SatelliteFieldStatistic row in rows)
                db.FieldMapLayers.Add(BuildSatelliteLayer(row, field.Name, boundary, now));
            source.Record!.MarkProcessed(now);
            AddEvent(db, tenant, actor, nameof(SatelliteFieldStatistic), source.Record.Id, source.Record.FarmId,
                "SatelliteMapped", "FieldIntelligence.SatelliteMapped", new { source.Record.Id, request.FieldId, request.BoundaryVersionId, Count = rows.Length }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/field-intelligence/fields/{request.FieldId}/satellite", rows.Select(ToSatellite).ToArray());
        }
        catch (ArgumentException ex) { return Bad("satellite_statistic_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("ingestion_record_already_mapped", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("satellite_statistic_conflict", DbMessage(ex)); }
    }


    private static async Task<IngestionSource> LoadIngestionSourceAsync(Guid recordId, IntegrationFeedType expected,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        IngestionRecord? record = await db.IngestionRecords.SingleOrDefaultAsync(x => x.Id == recordId, cancellationToken);
        if (record is null || !await scope.CanAccessFarmAsync(record.FarmId, cancellationToken)) return new(null, null, UseCaseResults.NotFound());
        if (record.Status != IngestionRecordStatus.Received)
            return new(record, null, Conflict("ingestion_record_already_decided", "Only a received ingestion record can be mapped."));
        IntegrationFeed? feed = await db.IntegrationFeeds.AsNoTracking().SingleOrDefaultAsync(x => x.Id == record.FeedId, cancellationToken);
        if (feed is null || feed.FeedType != expected)
            return new(record, feed, Bad("ingestion_feed_type_invalid", $"The ingestion record must originate from a {expected} feed."));
        return new(record, feed, null);
    }


    private static bool FeedAllowsField(IntegrationFeed feed, Guid? fieldId) =>
        feed.TargetType != IntegrationTargetType.Field || feed.TargetId == fieldId;


    private static FieldMapLayer BuildSatelliteLayer(SatelliteFieldStatistic row, string fieldName,
        FieldBoundaryVersion boundary, DateTimeOffset now)
    {
        string index = row.Index == VegetationIndex.Other ? row.CustomIndex! : row.Index.ToString().ToUpperInvariant();
        string geoJson = BoundaryGeoJson(boundary.Boundary, new { row.Id, row.FieldId, Index = index, row.Mean, row.Minimum, row.Maximum, row.ValidPixelPercent, row.CloudPercent, row.AcquiredAt });
        string color = row.Mean < 0.2m ? "#b91c1c" : row.Mean < 0.4m ? "#d97706" : row.Mean < 0.6m ? "#65a30d" : "#15803d";
        string style = JsonSerializer.Serialize(new { fillColor = color, fillOpacity = 0.55, strokeColor = "#ffffff", strokeWidth = 1 });
        return new FieldMapLayer(Guid.NewGuid(), row.FarmId, row.FieldId, row.BoundaryVersionId,
            MapLayerKind.SatelliteIndex, $"{fieldName} {index} {row.AcquiredAt:yyyy-MM-dd}",
            nameof(SatelliteFieldStatistic), row.Id, row.AcquiredAt, now, row.AcquiredAt, null,
            geoJson, style, row.Quality);
    }


    private static string BoundaryGeoJson(MultiPolygon boundary, object properties) =>
        JsonSerializer.Serialize(new { type = "Feature", geometry = MultiPolygonCoordinates(boundary), properties });


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

    private static SatelliteFieldStatisticResponse ToSatellite(SatelliteFieldStatistic x) => new(x.Id, x.FarmId,
        x.FieldId, x.BoundaryVersionId, x.FeedId, x.IngestionRecordId, x.ProviderAcquisitionId, x.Index,
        x.CustomIndex, x.AcquiredAt, x.ProcessedAt, x.Mean, x.Minimum, x.Maximum, x.StandardDeviation,
        x.ValidPixelPercent, x.CloudPercent, x.Quality);


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

    private sealed record IngestionSource(IngestionRecord? Record, IntegrationFeed? Feed, UseCaseResult? Error);

}
