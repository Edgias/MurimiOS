using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetRotation;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetRotation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetRotationEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapGet("/fields/{fieldId:guid}/rotation", async Task<IResult> (Guid fieldId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.GetRotation.GetRotationQuery(fieldId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningRead);

    }
}
