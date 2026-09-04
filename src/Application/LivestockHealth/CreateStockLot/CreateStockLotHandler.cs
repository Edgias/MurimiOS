using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using System.Text.Json;
using System.Security.Claims;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.CreateStockLot;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;
using VeterinaryMedicineStockLot = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot.VeterinaryMedicineStockLot;

public sealed class CreateStockLotHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<CreateStockLotCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateStockLotCommand message, CancellationToken cancellationToken) => CreateStockLotAsync(message.medicineId, message.request, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult> CreateStockLotAsync(
        Guid medicineId,
        CreateVeterinaryMedicineStockLotRequest request,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        CancellationToken cancellationToken)
    {
        VeterinaryMedicine? medicine = await db.VeterinaryMedicines.SingleOrDefaultAsync(x => x.Id == medicineId, cancellationToken);
        if (medicine is null || !await scopeAccess.CanAccessFarmAsync(medicine.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        try
        {
            var lot = new VeterinaryMedicineStockLot(
                Guid.NewGuid(), medicineId, request.BatchNumber, request.ReceivedOn, request.ExpiresOn,
                request.InitialQuantity, medicine.DisplayUnit);
            db.VeterinaryMedicineStockLots.Add(lot);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created(
                $"/api/v1/medicines/{medicineId}/stock-lots/{lot.Id}",
                new VeterinaryMedicineStockLotResponse(
                    lot.Id, lot.MedicineId, lot.BatchNumber, lot.ReceivedOn, lot.ExpiresOn,
                    lot.InitialQuantity, lot.QuantityOnHand, medicine.Unit,
                    lot.EnteredInitialQuantity, lot.EnteredUnit, lot.DisplayUnit));
        }
        catch (ArgumentException exception)
        {
            return UseCaseResults.BadRequest(new { code = "medicine_stock_lot_invalid", message = exception.Message });
        }
    }


}
