using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Traceability.ListDeliveries;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;

using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;

public sealed class ListDeliveriesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListDeliveriesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListDeliveriesQuery message, CancellationToken cancellationToken) => ListDeliveriesAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListDeliveriesAsync(Guid farmId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        DeliveryResponse[] rows = await db.Deliveries.Where(x => x.FarmId == farmId).OrderByDescending(x => x.DispatchedAt)
            .Select(x => new DeliveryResponse(x.Id, x.FarmId, x.DeliveryNumber, x.Recipient, x.Destination,
                x.Status, x.DispatchedAt, x.ReceivedAt, x.TransactionReference)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows);
    }


}
