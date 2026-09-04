using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Integrations;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Worker;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentStatus = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentStatus;
using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionAttemptOutcome = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttemptOutcome;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using MachineryTelemetryRecord = global::EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord.MachineryTelemetryRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;
using SensorValueQuality = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorValueQuality;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using MqttPayloadDecoder = global::EdgIQ.MurimiOS.Infrastructure.Integrations.MqttPayloadDecoder;
using MqttSubscriptionConfiguration = global::EdgIQ.MurimiOS.Infrastructure.Integrations.MqttSubscriptionConfiguration;
using MachineryTelemetryValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MachineryTelemetryValue;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;
using SensorReadingValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SensorReadingValue;

public sealed class MqttMessageProcessor(IServiceScopeFactory scopeFactory, TimeProvider timeProvider)
{
    public async Task ProcessAsync(MqttSubscriptionConfiguration subscription, string topic, string payload,
        CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<WorkerTenantContext>().Resolve(subscription.Descriptor.TenantId);
        PlatformDbContext db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        IntegrationFeed? feed = await db.IntegrationFeeds.SingleOrDefaultAsync(x => x.Id == subscription.Descriptor.FeedId
            && x.Status == IntegrationFeedStatus.Active, cancellationToken);
        if (feed is null) return;
        DateTimeOffset now = timeProvider.GetUtcNow();
        ProviderRecord source = feed.FeedType == IntegrationFeedType.Sensor
            ? MqttPayloadDecoder.DecodeSensor(topic, payload, subscription.FeedConfigurationJson, now,
                subscription.ProviderCode.Equals("the-things-stack", StringComparison.OrdinalIgnoreCase))
            : MqttPayloadDecoder.DecodeMachinery(topic, payload, subscription.FeedConfigurationJson, now);
        string digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
        if (await db.IngestionRecords.AnyAsync(x => x.FeedId == feed.Id && x.ExternalRecordId == source.ExternalRecordId
            && x.ContentSha256 == digest, cancellationToken)) return;

        Guid jobId = Guid.NewGuid(); Guid leaseId = Guid.NewGuid();
        var job = new IngestionJob(jobId, feed.Id, feed.FarmId, IngestionJobKind.Webhook,
            $"mqtt:{source.ExternalRecordId}:{digest[..16]}", now, "mqtt-subscription-worker", now, 3);
        job.Claim(leaseId, now, TimeSpan.FromMinutes(2));
        var record = new IngestionRecord(Guid.NewGuid(), jobId, feed.Id, feed.FarmId, source.ExternalRecordId, 1,
            source.ObservedAt, now, null, digest, payload, IngestionQuality.Valid);
        db.IngestionJobs.Add(job); db.IngestionRecords.Add(record);
        if (source.Normalized is SensorReadingValue sensor)
            await PersistSensorAsync(db, feed, record, sensor, source.ObservedAt, now, digest, cancellationToken);
        else if (source.Normalized is MachineryTelemetryValue machinery)
            await PersistMachineryAsync(db, feed, record, machinery, source.ObservedAt, now, digest, cancellationToken);
        else throw new InvalidOperationException("Unsupported MQTT normalized record.");
        record.MarkProcessed(now);
        job.Succeed(leaseId, now, JsonSerializer.Serialize(new { topic, source.ExternalRecordId }));
        feed.RecordSuccess(now, source.ObservedAt, JsonSerializer.Serialize(new { topic, source.ExternalRecordId }));
        db.IngestionAttempts.Add(new IngestionAttempt(Guid.NewGuid(), job.Id, job.Attempts, now, now,
            IngestionAttemptOutcome.Succeeded, null, 1, 0, null, null, null));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), subscription.Descriptor.TenantId, "integration.mqtt.message.ingested",
            JsonSerializer.Serialize(new { feedId = feed.Id, recordId = record.Id, topic, source.ExternalRecordId }), now));
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task PersistSensorAsync(PlatformDbContext db, IntegrationFeed feed, IngestionRecord record,
        SensorReadingValue value, DateTimeOffset observedAt, DateTimeOffset receivedAt, string digest, CancellationToken ct)
    {
        ConnectedDevice device = await db.ConnectedDevices.SingleOrDefaultAsync(x => x.FarmId == feed.FarmId
            && x.DeviceKey == value.DeviceKey && x.Status == ConnectedDeviceStatus.Active, ct)
            ?? throw new ProviderAdapterException("connected_device_missing", $"Active device '{value.DeviceKey}' was not found.", false);
        string[] keys = value.Channels.Select(x => x.ChannelKey).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        SensorChannel[] channels = await db.SensorChannels.Where(x => x.DeviceId == device.Id && x.IsActive && keys.Contains(x.ChannelKey)).ToArrayAsync(ct);
        var map = channels.ToDictionary(x => x.ChannelKey, StringComparer.OrdinalIgnoreCase);
        if (map.Count != keys.Length || value.Channels.Any(x => !map[x.ChannelKey].Unit.Equals(x.Unit, StringComparison.OrdinalIgnoreCase)))
            throw new ProviderAdapterException("sensor_channel_mapping_invalid", "MQTT channels must map to active device channels with matching units.", false);
        db.SensorReadings.AddRange(value.Channels.Select(item => new SensorReading(Guid.NewGuid(), feed.FarmId, device.Id,
            map[item.ChannelKey].Id, map[item.ChannelKey].MetricCode, item.Value, item.Unit, observedAt, receivedAt,
            map[item.ChannelKey].IsWithinRange(item.Value) ? SensorValueQuality.Valid : SensorValueQuality.Suspect,
            item.ExternalReadingId, digest, record.Id, item.Latitude, item.Longitude)));
        device.RecordSeen(observedAt, value.FirmwareVersion);
    }

    private static async Task PersistMachineryAsync(PlatformDbContext db, IntegrationFeed feed, IngestionRecord record,
        MachineryTelemetryValue value, DateTimeOffset observedAt, DateTimeOffset receivedAt, string digest, CancellationToken ct)
    {
        EquipmentAsset equipment = await db.EquipmentAssets.SingleOrDefaultAsync(x => x.FarmId == feed.FarmId
            && x.Code == value.EquipmentCode.ToUpper() && x.Status != EquipmentStatus.Retired, ct)
            ?? throw new ProviderAdapterException("equipment_missing", $"Equipment '{value.EquipmentCode}' was not found.", false);
        ConnectedDevice? device = null;
        if (!string.IsNullOrWhiteSpace(value.DeviceKey))
            device = await db.ConnectedDevices.SingleOrDefaultAsync(x => x.FarmId == feed.FarmId && x.EquipmentId == equipment.Id
                && x.DeviceKey == value.DeviceKey && x.Status == ConnectedDeviceStatus.Active, ct)
                ?? throw new ProviderAdapterException("machinery_device_missing", $"Device '{value.DeviceKey}' was not found.", false);
        db.MachineryTelemetry.Add(new MachineryTelemetryRecord(Guid.NewGuid(), feed.FarmId, equipment.Id, device?.Id,
            observedAt, receivedAt, value.EngineHours, value.SpeedKph, value.FuelLevelPercent, value.State,
            value.Latitude, value.Longitude, record.ExternalRecordId, digest, record.Id));
        device?.RecordSeen(observedAt, null);
    }
}
