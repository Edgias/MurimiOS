using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.GetRotation;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;

using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;

public sealed class GetRotationHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<GetRotationQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetRotationQuery message, CancellationToken cancellationToken) => GetRotationAsync(message.fieldId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> GetRotationAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.FarmFields.Where(x => x.Id == fieldId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        CropRotationResponse[] rows = await (from allocation in db.CropPlanAllocations.AsNoTracking()
            join plan in db.CropPlans.AsNoTracking() on allocation.PlanId equals plan.Id
            join season in db.GrowingSeasons.AsNoTracking() on plan.SeasonId equals season.Id
            where allocation.FieldId == fieldId && (plan.Status == CropPlanStatus.Approved || plan.Status == CropPlanStatus.Superseded)
            orderby season.StartsOn descending
            select new CropRotationResponse(plan.Id, plan.Revision, plan.Status, season.Id, season.Name,
                season.StartsOn, season.EndsOn, allocation.Crop, allocation.Variety, allocation.PlannedAreaHa,
                allocation.TargetYield, allocation.TargetYieldUnit)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows);
    }


}
