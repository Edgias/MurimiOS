using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Overview;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
public sealed record InsightFeedFreshnessResponse(Guid Id, IntegrationFeedType FeedType, string Name,
    IntegrationTargetType TargetType, Guid? TargetId, IntegrationFeedStatus Status,
    DateTimeOffset? LastObservedAt, DateTimeOffset? LastReceivedAt, DateTimeOffset? LastSucceededAt,
    DateTimeOffset? LastFailedAt, string? LastErrorCode, bool IsStale, int FreshnessThresholdMinutes);
