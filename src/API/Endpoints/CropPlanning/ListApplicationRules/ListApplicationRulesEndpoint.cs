using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListApplicationRules;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListApplicationRulesEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapGet("/inputs/{inputId:guid}/application-rules", async Task<IResult> (Guid inputId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.ListApplicationRules.ListApplicationRulesQuery(inputId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningRead);

    }
}
