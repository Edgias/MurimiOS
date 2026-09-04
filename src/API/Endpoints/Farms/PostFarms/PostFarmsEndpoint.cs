using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarms;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarms;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostFarmsEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {

        farms.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CreateFarmRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Farms.PostFarms.PostFarmsCommand(new global::EdgIQ.MurimiOS.Application.Farms.PostFarms.CreateFarmRequest(request.Id, request.Name, request.Code, request.CountryCode, request.TimeZone, request.ParentOrganisationNodeId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
