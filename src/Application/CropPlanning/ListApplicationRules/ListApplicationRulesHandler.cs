using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.ListApplicationRules;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;

public sealed class ListApplicationRulesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListApplicationRulesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListApplicationRulesQuery message, CancellationToken cancellationToken) => ListApplicationRulesAsync(message.inputId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListApplicationRulesAsync(Guid inputId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.AgriculturalInputs.Where(x => x.Id == inputId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        if (farmId is null || !await scope.CanAccessFarmAsync(farmId.Value, cancellationToken)) return UseCaseResults.NotFound();
        CropProductApplicationRule[] rows = await db.CropProductApplicationRules.AsNoTracking().Where(x => x.InputId == inputId)
            .OrderBy(x => x.Crop).ThenByDescending(x => x.EffectiveFrom).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToRule).ToArray());
    }

    private static ProductApplicationRuleResponse ToRule(CropProductApplicationRule x) => new(x.Id, x.InputId,
        x.Crop, x.Jurisdiction, x.MaximumRatePerHa, x.RateUnit, x.EnteredMaximumRatePerHa, x.EnteredRateUnit,
        x.ReEntryHours, x.PreHarvestIntervalDays, x.EffectiveFrom, x.EffectiveTo, x.AuthorizationReference);

}
