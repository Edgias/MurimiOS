using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.UpdateDraft;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.UpdateDraft;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class UpdateDraftEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapPut("/work-orders/{workOrderId:guid}/draft", async Task<IResult> (Guid workOrderId, UpdateWorkOrderDraftRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.UpdateDraft.UpdateDraftCommand(workOrderId, new global::EdgIQ.MurimiOS.Application.WorkOrder.UpdateDraft.UpdateWorkOrderDraftRequest(request.Title, request.WorkType, request.Priority, request.ScheduledStart, request.ScheduledEnd, request.Instructions)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsWrite);

    }
}
