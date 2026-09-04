using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;

public sealed record ProviderIngestionContext(
    Guid TenantId,
    Guid JobId,
    Guid FeedId,
    Guid FarmId,
    IntegrationFeedType FeedType,
    IntegrationTargetType TargetType,
    Guid? TargetId,
    string ExternalSourceId,
    string ConnectionConfigurationJson,
    string FeedConfigurationJson,
    string? CheckpointJson,
    string? CredentialSecret,
    DateTimeOffset? WindowStart,
    DateTimeOffset? WindowEnd,
    DateTimeOffset Now,
    ProviderSpatialTarget? SpatialTarget);
