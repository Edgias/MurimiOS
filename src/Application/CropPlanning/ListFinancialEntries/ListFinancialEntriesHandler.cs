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
namespace EdgIQ.MurimiOS.Application.CropPlanning.ListFinancialEntries;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanFinancialEntry = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanFinancialEntry;
using FinancialEntryType = global::EdgIQ.MurimiOS.Domain.CropPlan.FinancialEntryType;

public sealed class ListFinancialEntriesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListFinancialEntriesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListFinancialEntriesQuery message, CancellationToken cancellationToken) => ListFinancialEntriesAsync(message.planId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListFinancialEntriesAsync(Guid planId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        CropPlan? plan = await db.CropPlans.AsNoTracking().SingleOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null || !await scope.CanAccessFarmAsync(plan.FarmId, cancellationToken)) return UseCaseResults.NotFound();
        CropPlanFinancialEntry[] rows = await db.CropPlanFinancialEntries.AsNoTracking().Where(x => x.PlanId == planId)
            .OrderByDescending(x => x.OccurredAt).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToFinancial).ToArray());
    }

    private static CropPlanFinancialEntryResponse ToFinancial(CropPlanFinancialEntry x) => new(x.Id, x.PlanId,
        x.AllocationId, x.EntryType, x.Category, x.Amount, x.Currency, x.OccurredAt, x.Reference, x.Notes, x.RecordedBy);

}
