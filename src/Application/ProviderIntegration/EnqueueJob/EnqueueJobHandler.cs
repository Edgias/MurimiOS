using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.EnqueueJob;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class EnqueueJobHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<EnqueueJobCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(EnqueueJobCommand message, CancellationToken cancellationToken) => EnqueueJobAsync(message.feedId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> EnqueueJobAsync(Guid feedId, EnqueueIngestionJobRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        IntegrationFeed? feed = await FindFeedAsync(feedId, db, scope, cancellationToken); if (feed is null) return UseCaseResults.NotFound();
        if (feed.Status != IntegrationFeedStatus.Active) return Conflict("integration_feed_inactive", "Only an active feed can accept ingestion jobs.");
        IngestionJob? existing = await db.IngestionJobs.AsNoTracking().SingleOrDefaultAsync(x => x.FeedId == feedId && x.IdempotencyKey == request.IdempotencyKey, cancellationToken);
        if (existing is not null) return UseCaseResults.Ok(ToJob(existing));
        try
        {
            Guid id = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
            var job = new IngestionJob(id, feedId, feed.FarmId, request.Kind, request.IdempotencyKey, now,
                actor.Subject, request.AvailableAt ?? now, request.MaximumAttempts ?? 5, request.WindowStart, request.WindowEnd);
            db.IngestionJobs.Add(job);
            AddEvent(db, tenant, actor, nameof(IngestionJob), id, feed.FarmId, "JobQueued", "Integration.JobQueued",
                new { job.Id, job.FeedId, job.FarmId, job.Kind, job.IdempotencyKey, job.AvailableAt, job.WindowStart, job.WindowEnd }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/integrations/jobs/{id}", ToJob(job));
        }
        catch (ArgumentException ex) { return Bad("ingestion_job_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("ingestion_job_conflict", DbMessage(ex)); }
    }


    private static async Task<IntegrationFeed?> FindFeedAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken, bool noTracking = false)
    {
        IQueryable<IntegrationFeed> query = noTracking ? db.IntegrationFeeds.AsNoTracking() : db.IntegrationFeeds;
        IntegrationFeed? feed = await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return feed is not null && await scope.CanAccessFarmAsync(feed.FarmId, cancellationToken) ? feed : null;
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
