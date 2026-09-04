using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Sync.GetSyncPull;


public sealed partial class GetSyncPullHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetSyncPullQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetSyncPullQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.checkpoint, message.limit, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            long checkpoint,
            int? limit,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            int pageSize = Math.Clamp(limit ?? 200, 1, 500);
            ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
            IQueryable<EdgIQ.MurimiOS.Domain.SyncClient.SyncChange> query = db.SyncChanges;
            if (!scope.IsUnrestricted)
                query = query.Where(change => change.FarmId != null && scope.FarmIds!.Contains(change.FarmId.Value));
            var changes = await query
                .Where(x => x.Id > checkpoint)
                .OrderBy(x => x.Id)
                .Take(pageSize + 1)
                .ToArrayAsync(cancellationToken);
            bool hasMore = changes.Length > pageSize;

            SyncChangeResponse[] responseChanges = changes
                .Take(pageSize)
                .Select(x => new SyncChangeResponse(
                    x.Id,
                    x.EntityType,
                    x.EntityId,
                    x.Operation,
                    JsonSerializer.Deserialize<JsonElement>(x.Payload),
                    x.ChangedAt))
                .ToArray();
            long nextCheckpoint = responseChanges.LastOrDefault()?.Sequence ?? checkpoint;

            return UseCaseResults.Ok(new SyncPullResponse(nextCheckpoint, hasMore, responseChanges));
        }

}
