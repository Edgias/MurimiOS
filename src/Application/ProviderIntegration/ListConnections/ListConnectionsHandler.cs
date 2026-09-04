using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.ListConnections;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using IntegrationHealth = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationHealth;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;

public sealed class ListConnectionsHandler(IPlatformDbContext db) : IRequestHandler<ListConnectionsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListConnectionsQuery message, CancellationToken cancellationToken) => ListConnectionsAsync(message.category, message.status, db, cancellationToken);

    private static async Task<UseCaseResult> ListConnectionsAsync(IntegrationCategory? category,
        IntegrationConnectionStatus? status, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        IQueryable<IntegrationConnection> query = db.IntegrationConnections.AsNoTracking();
        if (category is { } type) query = query.Where(x => x.Category == type);
        if (status is { } state) query = query.Where(x => x.Status == state);
        IntegrationConnection[] rows = await query.OrderBy(x => x.Category).ThenBy(x => x.Name).ToArrayAsync(cancellationToken);
        Guid[] ids = rows.Select(x => x.Id).ToArray();
        var credentials = await db.IntegrationCredentials.AsNoTracking().Where(x => ids.Contains(x.ConnectionId) && x.Status == IntegrationCredentialStatus.Active)
            .Select(x => new ActiveCredentialProjection(x.ConnectionId, x.CredentialVersion, x.Fingerprint, x.ExpiresAt))
            .ToDictionaryAsync(x => x.ConnectionId, cancellationToken);
        var feedCounts = await db.IntegrationFeeds.AsNoTracking().Where(x => ids.Contains(x.ConnectionId) && x.Status != IntegrationFeedStatus.Disabled)
            .GroupBy(x => x.ConnectionId).Select(x => new { Id = x.Key, Count = x.Count() }).ToDictionaryAsync(x => x.Id, x => x.Count, cancellationToken);
        return UseCaseResults.Ok(rows.Select(x => ToConnection(x, credentials.GetValueOrDefault(x.Id), feedCounts.GetValueOrDefault(x.Id))).ToArray());
    }


    private static IntegrationConnectionResponse ToConnection(IntegrationConnection x, ActiveCredentialProjection? credential, int feedCount) =>
        new(x.Id, x.ProviderCode, x.Name, x.Category, x.Status, x.Health, x.CreatedAt, x.CreatedBy, x.ActivatedAt,
            x.LastSucceededAt, x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.RateLimitRemaining,
            x.RateLimitResetAt, x.DisabledAt, credential?.CredentialVersion, credential?.Fingerprint, credential?.ExpiresAt, feedCount, x.Version);

    private sealed record ActiveCredentialProjection(Guid ConnectionId, int CredentialVersion, string Fingerprint, DateTimeOffset? ExpiresAt);

}
