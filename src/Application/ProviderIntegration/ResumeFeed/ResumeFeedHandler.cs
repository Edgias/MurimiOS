using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ResumeFeed;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class ResumeFeedHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<ResumeFeedCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ResumeFeedCommand message, CancellationToken cancellationToken) => ResumeFeedAsync(message.feedId, db, scope, tenant, actor, clock, context, cancellationToken);
    private static Task<UseCaseResult> ResumeFeedAsync(Guid feedId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken) =>        ChangeFeedStatusAsync(feedId, "resume", db, scope, tenant, actor, clock, context, cancellationToken);
    private static async Task<UseCaseResult> ChangeFeedStatusAsync(Guid feedId, string command, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        IntegrationFeed? feed = await FindFeedAsync(feedId, db, scope, cancellationToken); if (feed is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        try { if (command == "pause") feed.Pause(); else if (command == "resume") feed.Resume(now); else feed.Disable(); }
        catch (InvalidOperationException ex) { return Conflict("integration_feed_transition", ex.Message); }
        string action = command == "pause" ? "FeedPaused" : command == "resume" ? "FeedResumed" : "FeedDisabled";
        AddEvent(db, tenant, actor, nameof(IntegrationFeed), feed.Id, feed.FarmId, action, $"Integration.{action}", new { feed.Id, feed.Status }, now, context);
        await db.SaveChangesAsync(cancellationToken); return UseCaseResults.Ok(ToFeed(feed, now));
    }


    private static async Task<IntegrationFeed?> FindFeedAsync(Guid id, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken, bool noTracking = false)
    {
        IQueryable<IntegrationFeed> query = noTracking ? db.IntegrationFeeds.AsNoTracking() : db.IntegrationFeeds;
        IntegrationFeed? feed = await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        return feed is not null && await scope.CanAccessFarmAsync(feed.FarmId, cancellationToken) ? feed : null;
    }

    private static IntegrationFeedResponse ToFeed(IntegrationFeed x, DateTimeOffset now) => new(x.Id, x.ConnectionId,
        x.FarmId, x.FeedType, x.TargetType, x.TargetId, x.Name, x.ExternalSourceId, x.PollingIntervalMinutes,
        x.FreshnessThresholdMinutes, x.Status, x.NextPollAt, x.LastObservedAt, x.LastReceivedAt, x.LastSucceededAt,
        x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.LastReceivedAt is null || now - x.LastReceivedAt > TimeSpan.FromMinutes(x.FreshnessThresholdMinutes),
        x.CheckpointJson is null ? null : ParseJson(x.CheckpointJson), x.Version);

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

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
