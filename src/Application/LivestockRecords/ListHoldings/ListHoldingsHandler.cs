using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockRecords.ListHoldings;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;

using LivestockHoldingType = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHoldingType;

public sealed class ListHoldingsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListHoldingsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListHoldingsQuery message, CancellationToken cancellationToken) => ListHoldingsAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListHoldingsAsync(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        LivestockHoldingResponse[] rows = await db.LivestockHoldings.Where(x => x.FarmId == farmId).OrderBy(x => x.Name)
            .Select(x => new LivestockHoldingResponse(x.Id, x.FarmId, x.Code, x.Name, x.Type)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows);
    }


}
