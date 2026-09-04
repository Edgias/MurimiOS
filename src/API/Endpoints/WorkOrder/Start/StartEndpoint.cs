using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Start;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class StartEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapPost("/work-orders/{workOrderId:guid}/start", async Task<IResult> (Guid workOrderId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.Start.StartCommand(workOrderId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsWrite);

    }
}
