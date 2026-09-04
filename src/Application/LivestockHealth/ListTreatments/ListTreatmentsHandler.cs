using System.Text.Json;
using System.Security.Claims;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockHealth.ListTreatments;


public sealed class ListTreatmentsHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ListTreatmentsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListTreatmentsQuery message, CancellationToken cancellationToken) => ListTreatmentsAsync(message.subjectId, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult> ListTreatmentsAsync(
        Guid subjectId, IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.LivestockSubjects.Where(x => x.Id == subjectId)
            .Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scopeAccess.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        LivestockTreatmentResponse[] results = await (
            from treatment in db.LivestockTreatments
            join stockLot in db.VeterinaryMedicineStockLots on treatment.MedicineStockLotId equals stockLot.Id
            join medicine in db.VeterinaryMedicines on stockLot.MedicineId equals medicine.Id
            where treatment.SubjectId == subjectId
            orderby treatment.AdministeredAt descending
            select new LivestockTreatmentResponse(
                treatment.Id, treatment.SubjectId, treatment.MedicineStockLotId, medicine.Name,
                stockLot.BatchNumber, treatment.Dose, treatment.Unit,
                treatment.EnteredDose, treatment.EnteredUnit, treatment.DisplayUnit, treatment.Route,
                treatment.AdministeredAt, treatment.MeatHoldUntil, treatment.MilkHoldUntil, treatment.Notes))
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(results);
    }


}
