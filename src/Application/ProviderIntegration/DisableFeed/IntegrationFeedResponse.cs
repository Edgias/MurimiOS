using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.DisableFeed;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
public sealed record IntegrationFeedResponse(Guid Id, Guid ConnectionId, Guid FarmId, IntegrationFeedType FeedType,
    IntegrationTargetType TargetType, Guid? TargetId, string Name, string ExternalSourceId,
    int PollingIntervalMinutes, int FreshnessThresholdMinutes, IntegrationFeedStatus Status,
    DateTimeOffset NextPollAt, DateTimeOffset? LastObservedAt, DateTimeOffset? LastReceivedAt,
    DateTimeOffset? LastSucceededAt, DateTimeOffset? LastFailedAt, string? LastErrorCode,
    string? LastErrorMessage, bool IsStale, JsonElement? Checkpoint, uint Version);
