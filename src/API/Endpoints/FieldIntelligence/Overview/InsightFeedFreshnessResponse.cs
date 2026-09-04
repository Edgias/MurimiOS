using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Overview;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
public sealed record InsightFeedFreshnessResponse(Guid Id, IntegrationFeedType FeedType, string Name,
    IntegrationTargetType TargetType, Guid? TargetId, IntegrationFeedStatus Status,
    DateTimeOffset? LastObservedAt, DateTimeOffset? LastReceivedAt, DateTimeOffset? LastSucceededAt,
    DateTimeOffset? LastFailedAt, string? LastErrorCode, bool IsStale, int FreshnessThresholdMinutes);
