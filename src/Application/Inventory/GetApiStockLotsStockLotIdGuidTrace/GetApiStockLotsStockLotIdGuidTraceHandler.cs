using EdgIQ.MurimiOS.Domain.InventoryMovement;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.GetApiStockLotsStockLotIdGuidTrace;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;

using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;

public sealed partial class GetApiStockLotsStockLotIdGuidTraceHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<GetApiStockLotsStockLotIdGuidTraceQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetApiStockLotsStockLotIdGuidTraceQuery message, CancellationToken cancellationToken) => ExecuteAsync(message.stockLotId, db, scopeAccess, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid stockLotId,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            CancellationToken cancellationToken)         {
            var stock = await (
                from lot in db.InputStockLots
                join input in db.AgriculturalInputs on lot.InputId equals input.Id
                where lot.Id == stockLotId
                select new { Lot = lot, InputName = input.Name, input.Unit, input.DisplayUnit, input.FarmId })
                .SingleOrDefaultAsync(cancellationToken);
            if (stock is null)
            {
                return UseCaseResults.NotFound();
            }
            if (!await scopeAccess.CanAccessFarmAsync(stock.FarmId, cancellationToken))
            {
                return UseCaseResults.NotFound();
            }

            InputApplicationTraceResponse[] applications = await (
                from movement in db.InventoryMovements
                join operation in db.FieldOperations on movement.OperationId equals (Guid?)operation.Id
                join zone in db.CropZones on operation.CropZoneId equals (Guid?)zone.Id
                where movement.StockLotId == stockLotId && movement.Type == InventoryMovementType.Issue
                orderby movement.OccurredAt
                select new InputApplicationTraceResponse(
                    movement.Id,
                    operation.Id,
                    zone.Id,
                    zone.Name,
                    operation.OperationType,
                    movement.Quantity,
                    movement.OccurredAt))
                .ToArrayAsync(cancellationToken);

            Guid[] cropZoneIds = applications.Select(x => x.CropZoneId).Distinct().ToArray();
            HarvestTraceResponse[] harvests = await db.HarvestLots
                .Where(x => cropZoneIds.Contains(x.CropZoneId))
                .OrderBy(x => x.HarvestedAt)
                .Select(x => new HarvestTraceResponse(
                    x.Id,
                    x.CropZoneId,
                    x.LotCode,
                    x.Quantity,
                    x.Unit,
                    x.EnteredQuantity,
                    x.EnteredUnit,
                    x.DisplayUnit,
                    x.HarvestedAt))
                .ToArrayAsync(cancellationToken);

            return UseCaseResults.Ok(new StockLotTraceResponse(
                stock.Lot.Id,
                stock.Lot.LotCode,
                stock.InputName,
                stock.Unit,
                stock.DisplayUnit,
                stock.Lot.QuantityOnHand,
                applications,
                harvests));
        }

}
