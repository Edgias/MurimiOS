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
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.GetConnection;
using IntegrationCategory = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCategory;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using IntegrationHealth = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationHealth;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;

public sealed class GetConnectionHandler(IPlatformDbContext db, TimeProvider clock) : IRequestHandler<GetConnectionQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetConnectionQuery message, CancellationToken cancellationToken) => GetConnectionAsync(message.connectionId, db, clock, cancellationToken);

    private static async Task<UseCaseResult> GetConnectionAsync(Guid connectionId, IPlatformDbContext db,
        TimeProvider clock, CancellationToken cancellationToken)
    {
        IntegrationConnection? connection = await db.IntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == connectionId, cancellationToken);
        if (connection is null) return UseCaseResults.NotFound();
        IntegrationCredential[] credentials = await db.IntegrationCredentials.AsNoTracking().Where(x => x.ConnectionId == connectionId)
            .OrderByDescending(x => x.CredentialVersion).ToArrayAsync(cancellationToken);
        IntegrationFeed[] feeds = await db.IntegrationFeeds.AsNoTracking().Where(x => x.ConnectionId == connectionId).OrderBy(x => x.Name).ToArrayAsync(cancellationToken);
        var active = credentials.FirstOrDefault(x => x.Status == IntegrationCredentialStatus.Active);
        return UseCaseResults.Ok(new IntegrationConnectionDetailResponse(ToConnection(connection,
                active is null ? null : new ActiveCredentialProjection(active.ConnectionId, active.CredentialVersion, active.Fingerprint, active.ExpiresAt),
                feeds.Count(x => x.Status != IntegrationFeedStatus.Disabled)), ParseJson(connection.ConfigurationJson),
            credentials.Select(ToCredential).ToArray(), feeds.Select(x => ToFeed(x, clock.GetUtcNow())).ToArray()));
    }


    private static IntegrationConnectionResponse ToConnection(IntegrationConnection x, ActiveCredentialProjection? credential, int feedCount) =>
        new(x.Id, x.ProviderCode, x.Name, x.Category, x.Status, x.Health, x.CreatedAt, x.CreatedBy, x.ActivatedAt,
            x.LastSucceededAt, x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.RateLimitRemaining,
            x.RateLimitResetAt, x.DisabledAt, credential?.CredentialVersion, credential?.Fingerprint, credential?.ExpiresAt, feedCount, x.Version);

    private static IntegrationCredentialResponse ToCredential(IntegrationCredential x) => new(x.Id, x.ConnectionId,
        x.CredentialVersion, x.SecretProvider, x.Fingerprint, x.ValidFrom, x.ExpiresAt, x.CreatedAt, x.CreatedBy,
        x.PreviousCredentialId, x.Status, x.SupersededAt, x.SupersededByCredentialId, x.RevokedAt, x.RevokedBy);

    private static IntegrationFeedResponse ToFeed(IntegrationFeed x, DateTimeOffset now) => new(x.Id, x.ConnectionId,
        x.FarmId, x.FeedType, x.TargetType, x.TargetId, x.Name, x.ExternalSourceId, x.PollingIntervalMinutes,
        x.FreshnessThresholdMinutes, x.Status, x.NextPollAt, x.LastObservedAt, x.LastReceivedAt, x.LastSucceededAt,
        x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.LastReceivedAt is null || now - x.LastReceivedAt > TimeSpan.FromMinutes(x.FreshnessThresholdMinutes),
        x.CheckpointJson is null ? null : ParseJson(x.CheckpointJson), x.Version);

    private static JsonElement ParseJson(string json) => JsonSerializer.Deserialize<JsonElement>(json);

    private sealed record ActiveCredentialProjection(Guid ConnectionId, int CredentialVersion, string Fingerprint, DateTimeOffset? ExpiresAt);

}
