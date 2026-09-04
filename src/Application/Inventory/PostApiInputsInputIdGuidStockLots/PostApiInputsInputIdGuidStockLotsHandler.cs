using EdgIQ.MurimiOS.Domain.InventoryMovement;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Inventory.PostApiInputsInputIdGuidStockLots;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using InventoryMovementType = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovementType;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;

public sealed partial class PostApiInputsInputIdGuidStockLotsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, TimeProvider timeProvider) : IRequestHandler<PostApiInputsInputIdGuidStockLotsCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PostApiInputsInputIdGuidStockLotsCommand message, CancellationToken cancellationToken) => ExecuteAsync(message.inputId, message.request, db, scopeAccess, timeProvider, cancellationToken);
private static async Task<UseCaseResult> ExecuteAsync(
            Guid inputId,
            CreateInputStockLotRequest request,
            IPlatformDbContext db,
            IOrganisationScopeAccess scopeAccess,
            TimeProvider timeProvider,
            CancellationToken cancellationToken)         {
            AgriculturalInput? input = await db.AgriculturalInputs.SingleOrDefaultAsync(x => x.Id == inputId, cancellationToken);
            if (input is null || !await scopeAccess.CanAccessFarmAsync(input.FarmId, cancellationToken))
            {
                return UseCaseResults.NotFound();
            }

            try
            {
                var lot = new InputStockLot(
                    Guid.NewGuid(), inputId, request.LotCode, request.ReceivedOn, request.ExpiresOn,
                    request.InitialQuantity, input.DisplayUnit);
                var receipt = new InventoryMovement(
                    Guid.NewGuid(), lot.Id, InventoryMovementType.Receipt, request.InitialQuantity,
                    input.DisplayUnit, timeProvider.GetUtcNow(), null, null, "Initial stock receipt");
                db.InputStockLots.Add(lot);
                db.InventoryMovements.Add(receipt);
                await db.SaveChangesAsync(cancellationToken);

                return UseCaseResults.Created(
                    $"/api/v1/inputs/{inputId}/stock-lots/{lot.Id}",
                    new InputStockLotResponse(
                        lot.Id, inputId, lot.LotCode, lot.ReceivedOn, lot.ExpiresOn,
                        lot.InitialQuantity, lot.QuantityOnHand, input.Unit,
                        lot.EnteredInitialQuantity, lot.EnteredUnit, lot.DisplayUnit));
            }
            catch (ArgumentException exception)
            {
                return UseCaseResults.BadRequest(new { code = "input_stock_lot_invalid", message = exception.Message });
            }
        }

}
