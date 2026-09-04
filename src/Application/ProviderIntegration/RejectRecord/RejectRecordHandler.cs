using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.RejectRecord;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class RejectRecordHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<RejectRecordCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RejectRecordCommand message, CancellationToken cancellationToken) => RejectRecordAsync(message.recordId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);
    private static Task<UseCaseResult> RejectRecordAsync(Guid recordId, RejectIngestionRecordRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken) =>        DecideRecordAsync(recordId, request.Code, request.Reason, db, scope, tenant, actor, clock, context, cancellationToken);
    private static async Task<UseCaseResult> DecideRecordAsync(Guid recordId, string? code, string? reason,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        IngestionRecord? record = await db.IngestionRecords.SingleOrDefaultAsync(x => x.Id == recordId, cancellationToken);
        if (record is null || !await scope.CanAccessFarmAsync(record.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try { if (code is null) record.MarkProcessed(now); else record.Reject(now, code, reason!); }
        catch (ArgumentException ex) { return Bad("ingestion_record_decision_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("ingestion_record_already_decided", ex.Message); }
        string action = code is null ? "RecordProcessed" : "RecordRejected";
        AddEvent(db, tenant, actor, nameof(IngestionRecord), record.Id, record.FarmId, action, $"Integration.{action}",
            new { record.Id, record.JobId, record.FeedId, record.Status, record.RejectionCode }, now, context);
        await db.SaveChangesAsync(cancellationToken); return UseCaseResults.Ok(ToRecord(record, includePayload: false));
    }

    private static IngestionRecordResponse ToRecord(IngestionRecord x, bool includePayload) => new(x.Id, x.JobId, x.FeedId,
        x.FarmId, x.ExternalRecordId, x.SchemaVersion, x.ObservedAt, x.ReceivedAt, x.SourceModifiedAt, x.ContentSha256,
        x.Quality, x.Status, x.ProcessedAt, x.RejectionCode, x.RejectionReason, includePayload ? ParseJson(x.PayloadJson) : null);

    private static JsonElement ParseJson(string json) => JsonSerializer.Deserialize<JsonElement>(json);

    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string entityType, Guid entityId, Guid? farmId, string action, string eventType, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        if (farmId is { } farm) db.SyncChanges.Add(new SyncChange(tenant.TenantId, farm, entityType, entityId, action, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
