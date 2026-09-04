using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Farms.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Farms.GetFarms;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using FarmResponse = global::EdgIQ.MurimiOS.API.Endpoints.Farms.Shared.FarmResponse;
internal static class GetFarmsEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {

        farms.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Farms.GetFarms.GetFarmsQuery(), cancellationToken)).ToHttpResult()).Produces<FarmResponse[]>(200).RequireAuthorization(PlatformPermissions.FarmRead);

    }
}
