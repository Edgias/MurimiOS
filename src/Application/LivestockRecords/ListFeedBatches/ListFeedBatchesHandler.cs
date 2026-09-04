using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListFeedBatches;


public sealed class ListFeedBatchesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListFeedBatchesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListFeedBatchesQuery message, CancellationToken cancellationToken) => ListFeedBatchesAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListFeedBatchesAsync(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        LivestockFeedBatchResponse[] rows = await db.LivestockFeedBatches.Where(x => x.FarmId == farmId)
            .OrderByDescending(x => x.ReceivedOn).Select(x => new LivestockFeedBatchResponse(x.Id, x.FarmId, x.BatchCode,
                x.FeedName, x.InitialQuantity, x.QuantityOnHand, x.Unit, x.EnteredInitialQuantity, x.EnteredUnit,
                x.ReceivedOn, x.ExpiresOn, x.Supplier)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows);
    }


}
