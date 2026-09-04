using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Assign;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Assign;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class AssignEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapPost("/work-orders/{workOrderId:guid}/assignments", async Task<IResult> (Guid workOrderId, AssignWorkOrderRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.Assign.AssignCommand(workOrderId, new global::EdgIQ.MurimiOS.Application.WorkOrder.Assign.AssignWorkOrderRequest(request.Id, request.MembershipId, request.AssignmentRole, request.IsPrimary)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsAssign);

    }
}
