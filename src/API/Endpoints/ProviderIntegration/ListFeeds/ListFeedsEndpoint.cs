using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ListFeeds;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListFeedsEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {

        integrations.MapGet("/feeds", async Task<IResult> (Guid? farmId, Guid? connectionId, IntegrationFeedType? feedType, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.ListFeeds.ListFeedsQuery(farmId, connectionId, feedType), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsRead);

    }
}
