using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ListJobs;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

public sealed class ListJobsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListJobsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListJobsQuery message, CancellationToken cancellationToken) => ListJobsAsync(message.feedId, message.status, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListJobsAsync(Guid feedId, IngestionJobStatus? status, int? limit,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        IntegrationFeed? feed = await FindFeedAsync(feedId, db, scope, cancellationToken, noTracking: true); if (feed is null) return UseCaseResults.NotFound();
        IQueryable<IngestionJob> query = db.IngestionJobs.AsNoTracking().Where(x => x.FeedId == feedId);
        if (status is { } state) query = query.Where(x => x.Status == state);
        IngestionJob[] rows = await query.OrderByDescending(x => x.RequestedAt).Take(Math.Clamp(limit ?? 100, 1, 500)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToJob).ToArray());
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

}
