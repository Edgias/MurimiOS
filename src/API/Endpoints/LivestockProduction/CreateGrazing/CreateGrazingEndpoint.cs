using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateGrazing;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.CreateGrazing;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateGrazingEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapPost("/grazing-plans", async Task<IResult> (Guid farmId, CreateGrazingPlanRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateGrazing.CreateGrazingCommand(farmId, new global::EdgIQ.MurimiOS.Application.LivestockProduction.CreateGrazing.CreateGrazingPlanRequest(request.GroupSubjectId, request.HoldingId, request.StartsAt, request.EndsAt, request.TargetHeadCount, request.RestUntil, request.AvailableDryMatterKg, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
