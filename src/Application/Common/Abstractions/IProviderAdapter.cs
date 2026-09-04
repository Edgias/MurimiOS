using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;

public interface IProviderAdapter
{
    string ProviderCode { get; }
    IntegrationFeedType FeedType { get; }
    Task<ProviderIngestionResult> IngestAsync(ProviderIngestionContext context, CancellationToken cancellationToken);
}
