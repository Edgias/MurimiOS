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
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ReplayJob;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ReplayJobHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReplayJobCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReplayJobCommand message, CancellationToken cancellationToken) => ReplayJobAsync(message.jobId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ReplayJobAsync(Guid jobId, ReplayIngestionJobRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        IngestionJob? source = await FindJobAsync(jobId, db, scope, cancellationToken, noTracking: true); if (source is null) return UseCaseResults.NotFound();
        if (source.Status is not (IngestionJobStatus.DeadLettered or IngestionJobStatus.Succeeded or IngestionJobStatus.Cancelled)) return Conflict("ingestion_job_not_replayable", "Only a terminal job can be replayed.");
        Guid id = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
        try
        {
            var replay = new IngestionJob(id, source.FeedId, source.FarmId, IngestionJobKind.Replay,
                request.IdempotencyKey, now, actor.Subject, request.AvailableAt ?? now, request.MaximumAttempts ?? source.MaximumAttempts,
                request.WindowStart ?? source.WindowStart, request.WindowEnd ?? source.WindowEnd, source.Id);
            db.IngestionJobs.Add(replay);
            AddEvent(db, tenant, actor, nameof(IngestionJob), replay.Id, replay.FarmId, "JobReplayed", "Integration.JobReplayed",
                new { replay.Id, replay.PreviousJobId, replay.FeedId, replay.IdempotencyKey }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/integrations/jobs/{replay.Id}", ToJob(replay));
        }
        catch (ArgumentException ex) { return Bad("ingestion_replay_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("ingestion_replay_conflict", DbMessage(ex)); }
    }

    private static async Task<IngestionJob?> FindJobAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken, bool noTracking = false)
    {
        IQueryable<IngestionJob> query = noTracking ? db.IngestionJobs.AsNoTracking() : db.IngestionJobs;
        IngestionJob? job = await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return job is not null && await scope.CanAccessFarmAsync(job.FarmId, cancellationToken) ? job : null;
    }

    private static IngestionJobResponse ToJob(IngestionJob x) => new(x.Id, x.FeedId, x.FarmId, x.Kind,
        x.IdempotencyKey, x.Status, x.RequestedAt, x.RequestedBy, x.AvailableAt, x.Attempts, x.MaximumAttempts,
        x.LeasedUntil, x.LastClaimedAt, x.CompletedAt, x.WindowStart, x.WindowEnd, x.PreviousJobId,
        x.ResultCheckpointJson is null ? null : ParseJson(x.ResultCheckpointJson), x.LastErrorCode, x.LastErrorMessage, x.Version);

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

    private static Guid NonEmptyOrNew(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string DbMessage(DbUpdateException ex) => ex.InnerException?.Message ?? ex.Message;

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
