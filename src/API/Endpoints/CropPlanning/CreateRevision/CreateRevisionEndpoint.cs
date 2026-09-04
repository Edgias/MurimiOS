using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateRevision;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateRevision;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRevisionEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapPost("/plans/{planId:guid}/revisions", async Task<IResult> (Guid planId, CreateCropPlanRevisionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.CreateRevision.CreateRevisionCommand(planId, new global::EdgIQ.MurimiOS.Application.CropPlanning.CreateRevision.CreateCropPlanRevisionRequest(request.Id, request.Name)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningWrite);

    }
}
