using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Integrations;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Worker;
using IntegrationConnectionStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnectionStatus;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using MqttSubscriptionDescriptor = global::EdgIQ.MurimiOS.Infrastructure.Integrations.MqttSubscriptionDescriptor;
using MqttSubscriptionConfiguration = global::EdgIQ.MurimiOS.Infrastructure.Integrations.MqttSubscriptionConfiguration;

public sealed class MqttSubscriptionLoader(IServiceScopeFactory scopeFactory)
{
    public async Task<MqttSubscriptionConfiguration> LoadAsync(MqttSubscriptionDescriptor descriptor,
        CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<WorkerTenantContext>().Resolve(descriptor.TenantId);
        PlatformDbContext db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        var row = await (from feed in db.IntegrationFeeds.AsNoTracking()
            join connection in db.IntegrationConnections.AsNoTracking() on feed.ConnectionId equals connection.Id
            join credential in db.IntegrationCredentials.AsNoTracking() on connection.Id equals credential.ConnectionId
            where feed.Id == descriptor.FeedId && connection.Id == descriptor.ConnectionId
                && feed.Status == IntegrationFeedStatus.Active && connection.Status == IntegrationConnectionStatus.Active
                && credential.Id == descriptor.CredentialId && credential.Status == IntegrationCredentialStatus.Active
            select new { connection.ProviderCode, ConnectionJson = connection.ConfigurationJson,
                FeedJson = feed.ConfigurationJson, credential.ProtectedReference }).SingleOrDefaultAsync(cancellationToken);
        if (row is null) throw new InvalidOperationException("The MQTT subscription is no longer active.");
        return new MqttSubscriptionConfiguration(descriptor, row.ProviderCode, row.ConnectionJson, row.FeedJson, row.ProtectedReference);
    }
}
