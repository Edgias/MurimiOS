using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.LivestockProduction.ListGrazing;
using LivestockGrazingPlan = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.LivestockGrazingPlan;

public sealed class ListGrazingHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListGrazingQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListGrazingQuery message, CancellationToken cancellationToken) => ListGrazingAsync(message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListGrazingAsync(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken)) return UseCaseResults.NotFound();
        LivestockGrazingPlan[] rows = await db.LivestockGrazingPlans.AsNoTracking().Where(x => x.FarmId == farmId)
            .OrderByDescending(x => x.StartsAt).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(GrazingResponse).ToArray());
    }

    private static object GrazingResponse(LivestockGrazingPlan x) => new { x.Id, x.FarmId, x.GroupSubjectId, x.HoldingId,
        x.StartsAt, x.EndsAt, x.TargetHeadCount, x.RestUntil, x.AvailableDryMatterKg,
        DryMatterKgPerHead = x.AvailableDryMatterKg / x.TargetHeadCount,
        PlannedDays = decimal.Round((decimal)(x.EndsAt - x.StartsAt).TotalDays, 2), x.Status, x.ClosedAt, x.Notes };

}
