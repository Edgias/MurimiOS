using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.FailJob;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionAttemptOutcome = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttemptOutcome;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class FailJobHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<FailJobCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(FailJobCommand message, CancellationToken cancellationToken) => FailJobAsync(message.jobId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> FailJobAsync(Guid jobId, FailIngestionJobRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        IngestionJob? job = await FindJobAsync(jobId, db, scope, cancellationToken); if (job is null) return UseCaseResults.NotFound();
        IntegrationFeed feed = await db.IntegrationFeeds.SingleAsync(x => x.Id == job.FeedId, cancellationToken);
        IntegrationConnection connection = await db.IntegrationConnections.SingleAsync(x => x.Id == feed.ConnectionId, cancellationToken);
        DateTimeOffset now = clock.GetUtcNow(); DateTimeOffset retryAt = request.RetryAt ?? now.AddMinutes(Math.Min(60, Math.Pow(2, job.Attempts)));
        try
        {
            DateTimeOffset startedAt = job.LastClaimedAt ?? now; int attempt = job.Attempts;
            job.Fail(request.LeaseId, now, retryAt, request.ErrorCode, request.ErrorMessage);
            bool dead = job.Status == IngestionJobStatus.DeadLettered;
            feed.RecordFailure(now, request.ErrorCode, request.ErrorMessage, dead ? now.AddYears(100) : retryAt);
            connection.RecordFailure(now, request.ErrorCode, request.ErrorMessage, dead);
            db.IngestionAttempts.Add(new IngestionAttempt(Guid.NewGuid(), job.Id, attempt, startedAt, now,
                dead ? IngestionAttemptOutcome.PermanentFailure : IngestionAttemptOutcome.RetriableFailure,
                request.HttpStatusCode, request.RequestUnits, request.Cost, request.Currency, request.ErrorCode, request.ErrorMessage));
            AddEvent(db, tenant, actor, nameof(IngestionJob), job.Id, job.FarmId, dead ? "JobDeadLettered" : "JobRetryScheduled",
                dead ? "Integration.JobDeadLettered" : "Integration.JobRetryScheduled",
                new { job.Id, job.Status, job.Attempts, job.AvailableAt, request.ErrorCode, request.ErrorMessage }, now, context);
            await db.SaveChangesAsync(cancellationToken); return UseCaseResults.Ok(ToJob(job));
        }
        catch (ArgumentException ex) { return Bad("ingestion_failure_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("ingestion_lease_invalid", ex.Message); }
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

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
