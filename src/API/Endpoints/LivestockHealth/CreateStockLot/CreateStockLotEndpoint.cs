using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Claims;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.LivestockHealthEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateStockLot;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateStockLotEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/medicines/{medicineId:guid}/stock-lots", async Task<IResult> (Guid medicineId, CreateVeterinaryMedicineStockLotRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockHealth.CreateStockLot.CreateStockLotCommand(medicineId, new global::EdgIQ.MurimiOS.Application.LivestockHealth.CreateStockLot.CreateVeterinaryMedicineStockLotRequest(request.BatchNumber, request.ReceivedOn, request.ExpiresOn, request.InitialQuantity)), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
