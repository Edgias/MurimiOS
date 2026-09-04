using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Reject;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Reject;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using ReasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Shared.ReasonRequest;
internal static class RejectEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapPost("/work-orders/{workOrderId:guid}/reject", async Task<IResult> (Guid workOrderId, ReasonRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.Reject.RejectCommand(workOrderId, new global::EdgIQ.MurimiOS.Application.WorkOrder.Reject.ReasonRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsVerify);

    }
}
