using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Claims;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.LivestockHealthEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.CreateMedicine;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateMedicineEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/farms/{farmId:guid}/medicines", async Task<IResult> (Guid farmId, CreateVeterinaryMedicineRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockHealth.CreateMedicine.CreateMedicineCommand(farmId, new global::EdgIQ.MurimiOS.Application.LivestockHealth.CreateMedicine.CreateVeterinaryMedicineRequest(request.Name, request.ActiveIngredient, request.Unit, request.MeatWithdrawalDays, request.MilkWithdrawalHours)), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
