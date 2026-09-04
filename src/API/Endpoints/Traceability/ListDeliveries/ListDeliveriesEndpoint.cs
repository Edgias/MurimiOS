using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.ListDeliveries;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListDeliveriesEndpoint
{
    internal static void Map(IEndpointRouteBuilder deliveries)
    {
        deliveries.MapGet("/farms/{farmId:guid}", async Task<IResult> (Guid farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.ListDeliveries.ListDeliveriesQuery(farmId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityRead);

    }
}
