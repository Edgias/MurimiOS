using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateFollowUpWorkOrder;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateFollowUpWorkOrder;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateFollowUpWorkOrderEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/alerts/{alertId:guid}/work-orders", async Task<IResult> (Guid alertId, CreateAlertWorkOrderRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.CreateFollowUpWorkOrder.CreateFollowUpWorkOrderCommand(alertId, new global::EdgIQ.MurimiOS.Application.FieldIntelligence.CreateFollowUpWorkOrder.CreateAlertWorkOrderRequest(request.Id, request.Number, request.Title, request.WorkType, request.Priority, request.ScheduledStart, request.ScheduledEnd, request.Instructions, request.PlanImmediately)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.FieldIntelligenceWrite, PlatformPermissions.OperationsWrite);

    }
}
