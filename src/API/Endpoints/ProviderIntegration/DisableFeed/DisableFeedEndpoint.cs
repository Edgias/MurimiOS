using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.DisableFeed;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class DisableFeedEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/feeds/{feedId:guid}/disable", async Task<IResult> (Guid feedId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.DisableFeed.DisableFeedCommand(feedId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsManage);

    }
}
