using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CreateFeed;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
public sealed record CreateIntegrationFeedRequest(Guid? Id, Guid ConnectionId, IntegrationFeedType FeedType,
    IntegrationTargetType TargetType, Guid? TargetId, string Name, string ExternalSourceId,
    int PollingIntervalMinutes, int FreshnessThresholdMinutes, JsonElement Configuration, DateTimeOffset? NextPollAt);
