using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.ReceiveDelivery;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.ReceiveDelivery;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReceiveDeliveryEndpoint
{
    internal static void Map(IEndpointRouteBuilder deliveries)
    {
        deliveries.MapPost("/{deliveryId:guid}/receive", async Task<IResult> (Guid deliveryId, ReceiveDeliveryRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.ReceiveDelivery.ReceiveDeliveryCommand(deliveryId, new global::EdgIQ.MurimiOS.Application.Traceability.ReceiveDelivery.ReceiveDeliveryRequest(request.ReceivedAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityWrite);

    }
}
