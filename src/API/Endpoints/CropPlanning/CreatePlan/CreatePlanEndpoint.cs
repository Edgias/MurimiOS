using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreatePlan;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreatePlan;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreatePlan;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreatePlanEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapPost("/farms/{farmId:guid}/plans", async Task<IResult> (Guid farmId, CreateCropPlanRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.CreatePlan.CreatePlanCommand(farmId, new global::EdgIQ.MurimiOS.Application.CropPlanning.CreatePlan.CreateCropPlanRequest(request.Id, request.SeasonId, request.PlanningGroupId, request.Name, request.ScenarioName, request.Currency)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningWrite);

    }
}
