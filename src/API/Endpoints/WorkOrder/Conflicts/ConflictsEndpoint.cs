using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkOrder.Conflicts;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class ConflictsEndpoint
{
    internal static void Map(IEndpointRouteBuilder operations)
    {
        operations.MapGet("/conflicts", async Task<IResult> (Guid farmId, DateTimeOffset from, DateTimeOffset to, Guid? membershipId, string? resourceReference, Guid? excludeWorkOrderId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkOrder.Conflicts.ConflictsQuery(farmId, from, to, membershipId, resourceReference, excludeWorkOrderId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.OperationsRead);

    }
}
