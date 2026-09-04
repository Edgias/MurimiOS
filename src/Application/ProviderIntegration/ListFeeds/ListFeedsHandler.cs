using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ListFeeds;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;

public sealed class ListFeedsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, TimeProvider clock) : IRequestHandler<ListFeedsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListFeedsQuery message, CancellationToken cancellationToken) => ListFeedsAsync(message.farmId, message.connectionId, message.feedType, db, scope, clock, cancellationToken);

    private static async Task<UseCaseResult> ListFeedsAsync(Guid? farmId, Guid? connectionId, IntegrationFeedType? feedType,
        IPlatformDbContext db, IOrganisationScopeAccess scope, TimeProvider clock, CancellationToken cancellationToken)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(cancellationToken);
        if (farmId is { } requested && !resolved.AllowsFarm(requested)) return UseCaseResults.NotFound();
        IQueryable<IntegrationFeed> query = db.IntegrationFeeds.AsNoTracking();
        if (farmId is { } farm) query = query.Where(x => x.FarmId == farm);
        else if (!resolved.IsUnrestricted) query = query.Where(x => resolved.FarmIds!.Contains(x.FarmId));
        if (connectionId is { } connection) query = query.Where(x => x.ConnectionId == connection);
        if (feedType is { } type) query = query.Where(x => x.FeedType == type);
        IntegrationFeed[] rows = await query.OrderBy(x => x.Name).ToArrayAsync(cancellationToken);
        DateTimeOffset now = clock.GetUtcNow(); return UseCaseResults.Ok(rows.Select(x => ToFeed(x, now)).ToArray());
    }

    private static IntegrationFeedResponse ToFeed(IntegrationFeed x, DateTimeOffset now) => new(x.Id, x.ConnectionId,
        x.FarmId, x.FeedType, x.TargetType, x.TargetId, x.Name, x.ExternalSourceId, x.PollingIntervalMinutes,
        x.FreshnessThresholdMinutes, x.Status, x.NextPollAt, x.LastObservedAt, x.LastReceivedAt, x.LastSucceededAt,
        x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.LastReceivedAt is null || now - x.LastReceivedAt > TimeSpan.FromMinutes(x.FreshnessThresholdMinutes),
        x.CheckpointJson is null ? null : ParseJson(x.CheckpointJson), x.Version);

    private static JsonElement ParseJson(string json) => JsonSerializer.Deserialize<JsonElement>(json);

}
