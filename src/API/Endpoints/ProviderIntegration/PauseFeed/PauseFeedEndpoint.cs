using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.PauseFeed;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PauseFeedEndpoint
{
    internal static void Map(IEndpointRouteBuilder integrations)
    {
        integrations.MapPost("/feeds/{feedId:guid}/pause", async Task<IResult> (Guid feedId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ProviderIntegration.PauseFeed.PauseFeedCommand(feedId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.IntegrationsManage);

    }
}
