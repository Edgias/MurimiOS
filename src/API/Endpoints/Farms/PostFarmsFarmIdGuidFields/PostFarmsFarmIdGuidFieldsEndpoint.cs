using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared;
using EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarmsFarmIdGuidFields;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarmsFarmIdGuidFields;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;
internal static class PostFarmsFarmIdGuidFieldsEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {

        farms.MapPost("/{farmId:guid}/fields", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid farmId, CreateFieldRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Farms.PostFarmsFarmIdGuidFields.PostFarmsFarmIdGuidFieldsCommand(farmId, new global::EdgIQ.MurimiOS.Application.Farms.PostFarmsFarmIdGuidFields.CreateFieldRequest(request.Id, request.BoundaryId, request.Name, request.ValidFrom, request.Exterior.Select(item => new global::EdgIQ.MurimiOS.Application.Farms.PostFarmsFarmIdGuidFields.CoordinateRequest(item.Longitude, item.Latitude)).ToArray(), request.Source)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
