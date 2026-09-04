using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ReceiveDelivery;
using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public sealed class ReceiveDeliveryHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ReceiveDeliveryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReceiveDeliveryCommand message, CancellationToken cancellationToken) => ReceiveDeliveryAsync(message.deliveryId, message.request, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ReceiveDeliveryAsync(Guid deliveryId, ReceiveDeliveryRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Delivery? delivery = await db.Deliveries.SingleOrDefaultAsync(x => x.Id == deliveryId, cancellationToken);
        if (delivery is null || !await scope.CanAccessFarmAsync(delivery.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        try { delivery.MarkReceived(request.ReceivedAt); }
        catch (InvalidOperationException ex) { return UseCaseResults.Conflict(new { code = "invalid_delivery_status", message = ex.Message }); }
        var receiptEvent = new TraceEvent(Guid.NewGuid(), delivery.FarmId, TraceEventType.Transaction, "receiving", "received",
            request.ReceivedAt, "destination", null, "DeliveryReceipt", delivery.Id, delivery.TransactionReference);
        db.TraceEvents.Add(receiptEvent);
        db.TraceEventObjects.Add(new TraceEventObject(Guid.NewGuid(), receiptEvent.Id, TraceObjectType.Delivery,
            delivery.Id, TraceObjectRole.Subject));
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(new DeliveryResponse(delivery.Id, delivery.FarmId, delivery.DeliveryNumber,
            delivery.Recipient, delivery.Destination, delivery.Status, delivery.DispatchedAt, delivery.ReceivedAt, delivery.TransactionReference));
    }


}
