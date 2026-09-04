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
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ReceiveRecords;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ReceiveRecordsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReceiveRecordsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReceiveRecordsCommand message, CancellationToken cancellationToken) => ReceiveRecordsAsync(message.jobId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ReceiveRecordsAsync(Guid jobId, ReceiveIngestionRecordsRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        IngestionJob? job = await FindJobAsync(jobId, db, scope, cancellationToken); if (job is null) return UseCaseResults.NotFound();
        if (job.Status != IngestionJobStatus.Running || job.LeaseId != request.LeaseId || job.LeasedUntil < clock.GetUtcNow()) return Conflict("ingestion_lease_invalid", "A current matching job lease is required.");
        if (request.Records is null || request.Records.Count is 0 or > 500) return Bad("ingestion_batch_invalid", "A batch must contain between 1 and 500 records.");
        DateTimeOffset now = clock.GetUtcNow(); int accepted = 0, duplicates = 0;
        try
        {
            var incoming = request.Records.Select(item =>
            {
                string payload = item.Payload.GetRawText(); string sha = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload))).ToLowerInvariant();
                return new { Item = item, Payload = payload, Sha = sha };
            }).ToArray();
            string[] externalIds = incoming.Select(x => x.Item.ExternalRecordId).Distinct().ToArray();
            var known = (await db.IngestionRecords.AsNoTracking()
                    .Where(x => x.FeedId == job.FeedId && externalIds.Contains(x.ExternalRecordId))
                    .Select(x => new { x.ExternalRecordId, x.ContentSha256 }).ToArrayAsync(cancellationToken))
                .Select(x => $"{x.ExternalRecordId}\n{x.ContentSha256}")
                .ToHashSet(StringComparer.Ordinal);
            foreach (var incomingRecord in incoming)
            {
                IngestionRecordRequest item = incomingRecord.Item;
                if (!known.Add($"{item.ExternalRecordId}\n{incomingRecord.Sha}")) { duplicates++; continue; }
                db.IngestionRecords.Add(new IngestionRecord(NonEmptyOrNew(item.Id), job.Id, job.FeedId, job.FarmId,
                    item.ExternalRecordId, item.SchemaVersion, item.ObservedAt, now, item.SourceModifiedAt,
                    incomingRecord.Sha, incomingRecord.Payload, item.Quality));
                accepted++;
            }
            AddEvent(db, tenant, actor, nameof(IngestionJob), job.Id, job.FarmId, "RecordsReceived", "Integration.RecordsReceived",
                new { job.Id, Accepted = accepted, Duplicates = duplicates }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Ok(new ReceiveIngestionRecordsResponse(accepted, duplicates));
        }
        catch (ArgumentException ex) { return Bad("ingestion_record_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("ingestion_record_conflict", DbMessage(ex)); }
    }

    private static async Task<IngestionJob?> FindJobAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken, bool noTracking = false)
    {
        IQueryable<IngestionJob> query = noTracking ? db.IngestionJobs.AsNoTracking() : db.IngestionJobs;
        IngestionJob? job = await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return job is not null && await scope.CanAccessFarmAsync(job.FarmId, cancellationToken) ? job : null;
    }

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

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string DbMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
