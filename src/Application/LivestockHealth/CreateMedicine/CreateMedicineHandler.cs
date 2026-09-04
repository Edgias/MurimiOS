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
namespace EdgIQ.MurimiOS.Application.LivestockHealth.CreateMedicine;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;

public sealed class CreateMedicineHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<CreateMedicineCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateMedicineCommand message, CancellationToken cancellationToken) => CreateMedicineAsync(message.farmId, message.request, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult> CreateMedicineAsync(
        Guid farmId,
        CreateVeterinaryMedicineRequest request,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        CancellationToken cancellationToken)
    {
        if (!await scopeAccess.CanAccessFarmAsync(farmId, cancellationToken)
            || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken)) return UseCaseResults.NotFound();
        try
        {
            var medicine = new VeterinaryMedicine(
                Guid.NewGuid(), farmId, request.Name, request.ActiveIngredient, request.Unit,
                request.MeatWithdrawalDays, request.MilkWithdrawalHours);
            db.VeterinaryMedicines.Add(medicine);
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/farms/{farmId}/medicines/{medicine.Id}", ToResponse(medicine));
        }
        catch (ArgumentException exception)
        {
            return UseCaseResults.BadRequest(new { code = "veterinary_medicine_invalid", message = exception.Message });
        }
    }


    private static VeterinaryMedicineResponse ToResponse(VeterinaryMedicine medicine) =>
        new(medicine.Id, medicine.FarmId, medicine.Name, medicine.ActiveIngredient, medicine.Unit, medicine.DisplayUnit,
            medicine.MeatWithdrawalDays, medicine.MilkWithdrawalHours);

}
