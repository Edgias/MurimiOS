using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListPlans;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListPlansEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapGet("/farms/{farmId:guid}/plans", async Task<IResult> (Guid farmId, Guid? seasonId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.ListPlans.ListPlansQuery(farmId, seasonId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningRead);

    }
}
