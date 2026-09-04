using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Calendar;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Calendar;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class CalendarEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapGet("/calendar", async Task<IResult> (Guid farmId, DateTimeOffset from, DateTimeOffset to, Guid? assigneeId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.Calendar.CalendarQuery(farmId, from, to, assigneeId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsRead);

    }
}
