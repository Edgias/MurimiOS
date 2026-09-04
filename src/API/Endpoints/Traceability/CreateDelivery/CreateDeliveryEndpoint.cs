using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateDelivery;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CreateDelivery;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateDeliveryEndpoint
{
    internal static void Map(IEndpointRouteBuilder deliveries)
    {
        deliveries.MapPost("/", async Task<IResult> (CreateDeliveryRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.CreateDelivery.CreateDeliveryCommand(new global::EdgIQ.MurimiOS.Application.Traceability.CreateDelivery.CreateDeliveryRequest(request.Id, request.DeliveryNumber, request.Recipient, request.Destination, request.DispatchedAt, request.TransactionReference, request.Lines.Select(item => new global::EdgIQ.MurimiOS.Application.Traceability.CreateDelivery.DeliverySourceLine(item.ObjectType, item.ObjectId, item.Quantity, item.Unit)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityWrite);

    }
}
