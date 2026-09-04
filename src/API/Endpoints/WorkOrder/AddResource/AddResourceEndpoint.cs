using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.AddResource;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.AddResource;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class AddResourceEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapPost("/work-orders/{workOrderId:guid}/resources", async Task<IResult> (Guid workOrderId, AddWorkOrderResourceRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.AddResource.AddResourceCommand(workOrderId, new global::EdgIQ.MurimiOS.Application.WorkOrder.AddResource.AddWorkOrderResourceRequest(request.Id, request.ResourceType, request.ResourceName, request.Quantity, request.Unit, request.UnitCostPerCanonicalUnit, request.Currency, request.AgriculturalInputId, request.StockLotId, request.ResourceReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsWrite);

    }
}
