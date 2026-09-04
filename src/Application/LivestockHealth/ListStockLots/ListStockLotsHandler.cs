using System.Text.Json;
using System.Security.Claims;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.ListStockLots;


public sealed class ListStockLotsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ListStockLotsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListStockLotsQuery message, CancellationToken cancellationToken) => ListStockLotsAsync(message.medicineId, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult> ListStockLotsAsync(
        Guid medicineId, IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.VeterinaryMedicines.Where(x => x.Id == medicineId)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        VeterinaryMedicineStockLotResponse[] results = await (
            from lot in db.VeterinaryMedicineStockLots
            join medicine in db.VeterinaryMedicines on lot.MedicineId equals medicine.Id
            where lot.MedicineId == medicineId
            orderby lot.ReceivedOn descending
            select new VeterinaryMedicineStockLotResponse(
                lot.Id, lot.MedicineId, lot.BatchNumber, lot.ReceivedOn, lot.ExpiresOn,
                lot.InitialQuantity, lot.QuantityOnHand, medicine.Unit,
                lot.EnteredInitialQuantity, lot.EnteredUnit, lot.DisplayUnit))
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(results);
    }


}
