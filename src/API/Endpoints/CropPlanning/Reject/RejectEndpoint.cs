using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Reject;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Reject;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RejectEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapPost("/plans/{planId:guid}/reject", async Task<IResult> (Guid planId, RejectCropPlanRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.Reject.RejectCommand(planId, new global::EdgIQ.MurimiOS.Application.CropPlanning.Reject.RejectCropPlanRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningApprove);

    }
}
