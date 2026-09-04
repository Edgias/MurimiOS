using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.Submit;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Submit;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class SubmitEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapPost("/plans/{planId:guid}/submit", async Task<IResult> (Guid planId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.Submit.SubmitCommand(planId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningWrite);

    }
}
