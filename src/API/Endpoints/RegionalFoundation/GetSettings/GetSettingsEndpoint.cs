using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.GetSettings;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetSettingsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/settings/regional", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.RegionalFoundation.GetSettings.GetSettingsQuery(), cancellationToken)).ToHttpResult())
            .WithTags("Settings")
            .RequireAuthorization(PlatformPermissions.SettingsRead);

    }
}
