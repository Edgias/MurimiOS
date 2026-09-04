using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiInputsInputIdGuidStockLots;


public sealed partial class GetApiInputsInputIdGuidStockLotsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiInputsInputIdGuidStockLotsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiInputsInputIdGuidStockLotsQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.inputId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid inputId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            Guid? farmId = await db.AgriculturalInputs
                .Where(input => input.Id == inputId)
                .Select(input => (Guid?)input.FarmId)
                .SingleOrDefaultAsync(cancellationToken);
            if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken))
                return UseCaseResults.NotFound();
            InputStockLotResponse[] results = await (
                from lot in db.InputStockLots
                join input in db.AgriculturalInputs on lot.InputId equals input.Id
                where lot.InputId == inputId
                orderby lot.ReceivedOn descending
                select new InputStockLotResponse(
                    lot.Id,
                    lot.InputId,
                    lot.LotCode,
                    lot.ReceivedOn,
                    lot.ExpiresOn,
                    lot.InitialQuantity,
                    lot.QuantityOnHand,
                    input.Unit,
                    lot.EnteredInitialQuantity,
                    lot.EnteredUnit,
                    lot.DisplayUnit))
                .ToArrayAsync(cancellationToken);
            return UseCaseResults.Ok(results);
        }

}
