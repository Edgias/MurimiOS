using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Access.Create;
using EdgIQ.MurimiOS.API.Endpoints.Organisation.Create;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Create;
using EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.Create;
using EdgIQ.MurimiOS.API.Endpoints.Notification.Create;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Create;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class CreateEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapPost("/farms/{farmId:guid}/work-orders", async Task<IResult> (Guid farmId, CreateWorkOrderRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.Create.CreateCommand(farmId, new global::EdgIQ.MurimiOS.Application.WorkOrder.Create.CreateWorkOrderRequest(request.Id, request.Number, request.Title, request.WorkType, request.Priority, request.ScheduledStart, request.ScheduledEnd, request.Instructions, request.FieldId, request.BoundaryVersionId, request.CropZoneId, request.LivestockSubjectId, request.CropPlanId, request.PlannedOperationId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsWrite);

    }
}
