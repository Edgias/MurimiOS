using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListFinancialEntries;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListFinancialEntriesEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapGet("/plans/{planId:guid}/financial-entries", async Task<IResult> (Guid planId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.ListFinancialEntries.ListFinancialEntriesQuery(planId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningRead);

    }
}
