using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.CreateFeed;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;

public sealed class CreateFeedHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateFeedCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateFeedCommand message, CancellationToken cancellationToken) => CreateFeedAsync(message.farmId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateFeedAsync(Guid farmId, CreateIntegrationFeedRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken) || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken)) return UseCaseResults.NotFound();
        IntegrationConnection? connection = await db.IntegrationConnections.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.ConnectionId && x.Status == IntegrationConnectionStatus.Active, cancellationToken);
        if (connection is null) return Bad("active_connection_required", "The feed requires an active integration connection.");
        if (request.TargetType == IntegrationTargetType.Field && request.TargetId is { } fieldId
            && !await db.FarmFields.AnyAsync(x => x.Id == fieldId && x.FarmId == farmId, cancellationToken)) return Bad("feed_target_invalid", "The target field must belong to this farm.");
        try
        {
            Guid id = NonEmptyOrNew(request.Id); DateTimeOffset now = clock.GetUtcNow();
            var feed = new IntegrationFeed(id, request.ConnectionId, farmId, request.FeedType, request.TargetType,
                request.TargetId, request.Name, request.ExternalSourceId, request.PollingIntervalMinutes,
                request.FreshnessThresholdMinutes, SafeConfiguration(request.Configuration), request.NextPollAt ?? now);
            db.IntegrationFeeds.Add(feed);
            AddEvent(db, tenant, actor, nameof(IntegrationFeed), id, farmId, "FeedCreated", "Integration.FeedCreated",
                new { feed.Id, feed.ConnectionId, feed.FarmId, feed.FeedType, feed.TargetType, feed.TargetId, feed.ExternalSourceId }, now, context);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/integrations/feeds/{id}", ToFeed(feed, now));
        }
        catch (ArgumentException ex) { return Bad("integration_feed_invalid", ex.Message); }
        catch (DbUpdateException ex) { return Conflict("integration_feed_conflict", DbMessage(ex)); }
    }

    private static IntegrationFeedResponse ToFeed(IntegrationFeed x, DateTimeOffset now) => new(x.Id, x.ConnectionId,
        x.FarmId, x.FeedType, x.TargetType, x.TargetId, x.Name, x.ExternalSourceId, x.PollingIntervalMinutes,
        x.FreshnessThresholdMinutes, x.Status, x.NextPollAt, x.LastObservedAt, x.LastReceivedAt, x.LastSucceededAt,
        x.LastFailedAt, x.LastErrorCode, x.LastErrorMessage, x.LastReceivedAt is null || now - x.LastReceivedAt > TimeSpan.FromMinutes(x.FreshnessThresholdMinutes),
        x.CheckpointJson is null ? null : ParseJson(x.CheckpointJson), x.Version);


    private static string SafeConfiguration(JsonElement configuration)
    {
        if (configuration.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null) return "{}";
        if (configuration.ValueKind != JsonValueKind.Object) throw new ArgumentException("Integration configuration must be a JSON object.");
        EnsureNoSensitiveKeys(configuration); string json = configuration.GetRawText();
        if (json.Length > 32_000) throw new ArgumentException("Integration configuration cannot exceed 32000 characters.");
        return json;
    }

    private static void EnsureNoSensitiveKeys(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (IsSensitiveConfigurationKey(property.Name)) throw new ArgumentException($"Configuration property '{property.Name}' is sensitive; store it through a credential reference.");
                EnsureNoSensitiveKeys(property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array) foreach (JsonElement item in element.EnumerateArray()) EnsureNoSensitiveKeys(item);
    }

    private static bool IsSensitiveConfigurationKey(string key)
    {
        string normalized = new(key.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        return SensitiveConfigurationKeys.Contains(key)
            || normalized is "token" or "password" or "secret" or "apikey" or "accesskey" or "privatekey" or "credential" or "credentials"
            || normalized.EndsWith("password", StringComparison.Ordinal)
            || normalized.EndsWith("secret", StringComparison.Ordinal)
            || normalized.EndsWith("apikey", StringComparison.Ordinal)
            || normalized.EndsWith("accesskey", StringComparison.Ordinal)
            || normalized.EndsWith("privatekey", StringComparison.Ordinal)
            || normalized.EndsWith("accesstoken", StringComparison.Ordinal)
            || normalized.EndsWith("refreshtoken", StringComparison.Ordinal)
            || normalized.EndsWith("authtoken", StringComparison.Ordinal)
            || normalized.EndsWith("bearertoken", StringComparison.Ordinal);
    }

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

    private static readonly HashSet<string> SensitiveConfigurationKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "secret", "password", "token", "apikey", "api_key", "accesskey", "access_key",
        "privatekey", "private_key", "clientsecret", "client_secret", "credential", "credentials"
    };

}
