using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.IngestReadings;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;
using SensorValueQuality = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorValueQuality;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class IngestReadingsHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<IngestReadingsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(IngestReadingsCommand message, CancellationToken cancellationToken) => IngestReadingsAsync(message.recordId, message.request, db, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> IngestReadingsAsync(Guid recordId, IngestSensorReadingsRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        IngestionRecord? record = await db.IngestionRecords.SingleOrDefaultAsync(x => x.Id == recordId && x.Status == IngestionRecordStatus.Received, ct);
        if (record is null) return UseCaseResults.NotFound();
        IntegrationFeed? feed = await db.IntegrationFeeds.AsNoTracking().SingleOrDefaultAsync(x => x.Id == record.FeedId && x.FeedType == IntegrationFeedType.Sensor, ct);
        if (feed is null) return Bad("sensor_feed_required", "The ingestion record must come from a sensor feed.");
        ConnectedDevice? device = await db.ConnectedDevices.SingleOrDefaultAsync(x => x.Id == request.DeviceId && x.FarmId == record.FarmId && x.Status == ConnectedDeviceStatus.Active, ct);
        if (device is null || feed.TargetType == IntegrationTargetType.Device && feed.TargetId != device.Id) return Bad("device_scope_invalid", "The active device does not match the ingestion feed.");
        IReadOnlyList<IngestSensorReadingItem> items = request.Readings ?? [];
        if (items.Count == 0 || items.Count > 10_000 || items.Any(x => x.ChannelId == Guid.Empty)) return Bad("readings_invalid", "Supply between 1 and 10,000 readings with channel identifiers.");
        Guid[] channelIds = items.Select(x => x.ChannelId).Distinct().ToArray();
        SensorChannel[] channels = await db.SensorChannels.Where(x => channelIds.Contains(x.Id) && x.DeviceId == device.Id && x.IsActive).ToArrayAsync(ct);
        if (channels.Length != channelIds.Length) return Bad("channel_scope_invalid", "Every reading channel must be active and belong to the device.");
        var channelById = channels.ToDictionary(x => x.Id);
        if (items.Any(x => !channelById[x.ChannelId].Unit.Equals(x.Unit?.Trim(), StringComparison.OrdinalIgnoreCase))) return Bad("reading_unit_mismatch", "Reading units must match their channel definitions.");
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            SensorReading[] readings = items.Select(x => new SensorReading(Id(x.Id), record.FarmId, device.Id, x.ChannelId,
                channelById[x.ChannelId].MetricCode, x.Value, x.Unit, x.ObservedAt, record.ReceivedAt,
                channelById[x.ChannelId].IsWithinRange(x.Value) ? SensorValueQuality.Valid : SensorValueQuality.Suspect,
                x.ExternalReadingId, record.ContentSha256, record.Id, x.Latitude, x.Longitude)).ToArray();
            db.SensorReadings.AddRange(readings); device.RecordSeen(readings.Max(x => x.ObservedAt), request.FirmwareVersion); record.MarkProcessed(now);
            AddRecords(db, tenant, actor, record.FarmId, "Ingested", new { recordId = record.Id, deviceId = device.Id, count = readings.Length }, "SensorReadingsIngested", context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/connected/farms/{record.FarmId}/sensor-readings", new { count = readings.Length, device.LastSeenAt });
        }
        catch (ArgumentException e) { return Bad("reading_invalid", e.Message); }
    }

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, Guid farmId, string action, object payload, string eventType, UseCaseContext context)
    { DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload); Guid entityId = payload.GetType().GetProperty("Id")?.GetValue(payload) is Guid id ? id : Guid.NewGuid(); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, payload.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, payload.GetType().Name, entityId, action, json, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
