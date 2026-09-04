using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CreateFeed;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CreateFeed;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateFeedEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/farms/{farmId:guid}/feeds", async Task<IResult> (Guid farmId, CreateIntegrationFeedRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.CreateFeed.CreateFeedCommand(farmId, new global::EdgIQ.MurimiOS.Application.ProviderIntegration.CreateFeed.CreateIntegrationFeedRequest(request.Id, request.ConnectionId, request.FeedType, request.TargetType, request.TargetId, request.Name, request.ExternalSourceId, request.PollingIntervalMinutes, request.FreshnessThresholdMinutes, request.Configuration, request.NextPollAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsManage);

    }
}
