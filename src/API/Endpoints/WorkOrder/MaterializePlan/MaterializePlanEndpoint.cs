using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.MaterializePlan;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.MaterializePlan;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class MaterializePlanEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapPost("/plans/{planId:guid}/materialize", async Task<IResult> (Guid planId, MaterializePlanRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.MaterializePlan.MaterializePlanCommand(planId, new global::EdgIQ.MurimiOS.Application.WorkOrder.MaterializePlan.MaterializePlanRequest(request.PlannedOperationIds, request.Priority)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsWrite);

    }
}
