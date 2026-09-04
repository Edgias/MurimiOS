using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.IngestTelemetry;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentStatus = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentStatus;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using MachineryTelemetryRecord = global::EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord.MachineryTelemetryRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class IngestTelemetryHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<IngestTelemetryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(IngestTelemetryCommand message, CancellationToken cancellationToken) => IngestTelemetryAsync(message.recordId, message.request, db, tenant, actor, context, cancellationToken);

    private static async Task<UseCaseResult> IngestTelemetryAsync(Guid recordId, IngestMachineryTelemetryRequest request,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        IngestionRecord? record = await db.IngestionRecords.SingleOrDefaultAsync(x => x.Id == recordId && x.Status == IngestionRecordStatus.Received, ct);
        if (record is null) return UseCaseResults.NotFound();
        if (!await db.IntegrationFeeds.AnyAsync(x => x.Id == record.FeedId && x.FeedType == IntegrationFeedType.Machinery, ct)) return Bad("machinery_feed_required", "The ingestion record must come from a machinery feed.");
        EquipmentAsset? equipment = await db.EquipmentAssets.SingleOrDefaultAsync(x => x.Id == request.EquipmentId && x.FarmId == record.FarmId && x.Status != EquipmentStatus.Retired, ct);
        if (equipment is null) return Bad("equipment_scope_invalid", "The equipment is unavailable for this farm.");
        if (request.DeviceId.HasValue && !await db.ConnectedDevices.AnyAsync(x => x.Id == request.DeviceId && x.EquipmentId == equipment.Id && x.Status == ConnectedDeviceStatus.Active, ct)) return Bad("device_scope_invalid", "The telemetry device is not active for this equipment.");
        try
        {
            var telemetry = new MachineryTelemetryRecord(Id(request.Id), record.FarmId, equipment.Id, request.DeviceId,
                record.ObservedAt, record.ReceivedAt, request.EngineHours, request.SpeedKph, request.FuelLevelPercent,
                request.State, request.Latitude, request.Longitude, record.ExternalRecordId, record.ContentSha256, record.Id);
            db.MachineryTelemetry.Add(telemetry); record.MarkProcessed(DateTimeOffset.UtcNow);
            AddRecords(db, tenant, actor, record.FarmId, "Ingested", telemetry, "MachineryTelemetryIngested", context);
            await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/connected/farms/{record.FarmId}/telemetry/{telemetry.Id}", telemetry);
        }
        catch (ArgumentException e) { return Bad("telemetry_invalid", e.Message); }
    }

    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, Guid farmId, string action, object payload, string eventType, UseCaseContext context)
    { DateTimeOffset now = DateTimeOffset.UtcNow; string json = JsonSerializer.Serialize(payload); Guid entityId = payload.GetType().GetProperty("Id")?.GetValue(payload) is Guid id ? id : Guid.NewGuid(); db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId, action, payload.GetType().Name, entityId, farmId, actor.ScopeOrganisationNodeId, json, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId)); db.SyncChanges.Add(new SyncChange(tenant.TenantId, farmId, payload.GetType().Name, entityId, action, json, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
