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
namespace EdgIQ.MurimiOS.Application.LivestockHealth.ListMedicines;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;

public sealed class ListMedicinesHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ListMedicinesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListMedicinesQuery message, CancellationToken cancellationToken) => ListMedicinesAsync(message.farmId, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult> ListMedicinesAsync(
        Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scopeAccess, CancellationToken cancellationToken)
    {
        if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        VeterinaryMedicineResponse[] results = await db.VeterinaryMedicines
            .Where(x => x.FarmId == farmId)
            .OrderBy(x => x.Name)
            .Select(x => ToResponse(x))
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(results);
    }


    private static VeterinaryMedicineResponse ToResponse(VeterinaryMedicine medicine) =>
        new(medicine.Id, medicine.FarmId, medicine.Name, medicine.ActiveIngredient, medicine.Unit, medicine.DisplayUnit,
            medicine.MeatWithdrawalDays, medicine.MilkWithdrawalHours);

}
