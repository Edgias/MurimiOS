using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Unassign;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class UnassignEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapDelete("/work-orders/{workOrderId:guid}/assignments/{assignmentId:guid}", async Task<IResult> (Guid workOrderId, Guid assignmentId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.Unassign.UnassignCommand(workOrderId, assignmentId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsAssign);

    }
}
