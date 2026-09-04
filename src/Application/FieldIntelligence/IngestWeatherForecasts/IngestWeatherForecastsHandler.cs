using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
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
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherForecasts;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using WeatherForecast = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherForecast;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

public sealed class IngestWeatherForecastsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<IngestWeatherForecastsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(IngestWeatherForecastsCommand message, CancellationToken cancellationToken) => IngestWeatherForecastsAsync(message.recordId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> IngestWeatherForecastsAsync(Guid recordId,
        IngestWeatherForecastsRequest request, IPlatformDbContext db, IOrganisationScopeAccess scope,
        ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context,
        CancellationToken cancellationToken)
    {
        var source = await LoadIngestionSourceAsync(recordId, IntegrationFeedType.Weather, db, scope, cancellationToken);
        if (source.Error is not null) return source.Error;
        if (request.Forecasts is null || request.Forecasts.Count is 0 or > 500)
            return Bad("weather_forecast_batch_invalid", "A forecast batch must contain between 1 and 500 periods.");
        if (request.FieldId is { } fieldId && !await db.FarmFields.AnyAsync(x => x.Id == fieldId && x.FarmId == source.Record!.FarmId, cancellationToken))
            return Bad("weather_field_invalid", "The weather field must belong to the ingestion farm.");
        if (!FeedAllowsField(source.Feed!, request.FieldId)) return Bad("weather_feed_target_invalid", "This feed is assigned to a different field.");
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            WeatherLocation location = await FindOrCreateLocationAsync(source.Record!.FarmId, request.FieldId,
                request.GridKey, request.Latitude, request.Longitude, request.ElevationMetres, now, db, cancellationToken);
            var rows = request.Forecasts.Select(item => new WeatherForecast(NonEmptyOrNew(item.Id), location.Id,
                source.Record.FarmId, request.FieldId, source.Record.FeedId, source.Record.Id, item.ProviderForecastId,
                item.IssuedAt, item.ValidFrom, item.ValidTo, item.MinimumTemperatureC, item.MaximumTemperatureC,
                item.PrecipitationMm, item.PrecipitationProbabilityPercent, item.WindSpeedMps,
                item.RelativeHumidityPercent, item.Quality)).ToArray();
            db.WeatherForecasts.AddRange(rows); source.Record.MarkProcessed(now);
            AddEvent(db, tenant, actor, nameof(WeatherForecast), source.Record.Id, source.Record.FarmId,
                "ForecastMapped", "FieldIntelligence.ForecastMapped", new { source.Record.Id, LocationId = location.Id, request.FieldId, Count = rows.Length }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/field-intelligence/fields/{request.FieldId}/weather", rows.Select(ToForecast).ToArray());
        }
        catch (ArgumentException ex) { return Bad("weather_forecast_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("ingestion_record_already_mapped", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("weather_forecast_conflict", DbMessage(ex)); }
    }


    private static async Task<WeatherLocation> FindOrCreateLocationAsync(Guid farmId, Guid? fieldId,
        string gridKey, decimal latitude, decimal longitude, decimal? elevationMetres, DateTimeOffset now,
        IPlatformDbContext db, CancellationToken cancellationToken)
    {
        string normalized = gridKey.Trim().ToLowerInvariant();
        WeatherLocation? existing = await db.WeatherLocations.SingleOrDefaultAsync(x => x.FarmId == farmId
            && x.FieldId == fieldId && x.GridKey == normalized, cancellationToken);
        if (existing is not null)
        {
            if (existing.Latitude != decimal.Round(latitude, 6) || existing.Longitude != decimal.Round(longitude, 6))
                throw new ArgumentException("The grid key is already registered at different coordinates.", nameof(gridKey));
            return existing;
        }
        var created = new WeatherLocation(Guid.NewGuid(), farmId, fieldId, normalized, latitude, longitude, elevationMetres, now);
        db.WeatherLocations.Add(created); return created;
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

    private static WeatherForecastResponse ToForecast(WeatherForecast x) => new(x.Id, x.LocationId, x.FarmId,
        x.FieldId, x.FeedId, x.IngestionRecordId, x.ProviderForecastId, x.IssuedAt, x.ValidFrom, x.ValidTo,
        x.MinimumTemperatureC, x.MaximumTemperatureC, x.PrecipitationMm, x.PrecipitationProbabilityPercent,
        x.WindSpeedMps, x.RelativeHumidityPercent, x.Quality);


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
