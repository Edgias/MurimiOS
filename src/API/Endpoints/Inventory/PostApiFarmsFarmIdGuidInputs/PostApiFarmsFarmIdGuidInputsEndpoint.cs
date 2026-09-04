using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiFarmsFarmIdGuidInputs;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostApiFarmsFarmIdGuidInputsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapPost("/farms/{farmId:guid}/inputs", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid farmId, CreateAgriculturalInputRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Inventory.PostApiFarmsFarmIdGuidInputs.PostApiFarmsFarmIdGuidInputsCommand(farmId, new global::EdgIQ.MurimiOS.Application.Inventory.PostApiFarmsFarmIdGuidInputs.CreateAgriculturalInputRequest(request.Name, request.Category, request.Unit)), cancellationToken)).ToHttpResult()).WithTags("Inventory").RequireAuthorization(PlatformPermissions.InventoryWrite);

    }
}
