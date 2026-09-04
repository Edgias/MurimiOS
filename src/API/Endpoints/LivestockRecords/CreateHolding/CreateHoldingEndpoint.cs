using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHolding;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHolding;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateHoldingEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/farms/{farmId:guid}/livestock/holdings", async Task<IResult> (Guid farmId, CreateLivestockHoldingRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateHolding.CreateHoldingCommand(farmId, new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateHolding.CreateLivestockHoldingRequest(request.Id, request.Code, request.Name, request.Type)), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
