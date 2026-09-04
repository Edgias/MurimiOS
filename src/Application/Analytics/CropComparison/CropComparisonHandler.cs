using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.CropComparison;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsFactKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFactKind;

public sealed class CropComparisonHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<CropComparisonQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CropComparisonQuery message, CancellationToken cancellationToken) => CropComparisonAsync(message.metricCode, message.from, message.to, message.farmId, db, scope, cancellationToken);

    private static async Task<UseCaseResult> CropComparisonAsync(string metricCode, DateOnly from, DateOnly to, Guid? farmId,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    { ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct); IQueryable<AnalyticsFact> query = global::EdgIQ.MurimiOS.Application.Common.Analytics.AnalyticsFacts.CurrentFacts(db).Where(x => x.Kind == AnalyticsFactKind.Crop && x.MetricCode == metricCode.ToLower() && x.PeriodEnd >= from && x.PeriodStart <= to); if (farmId.HasValue) { if (!resolved.AllowsFarm(farmId.Value)) return UseCaseResults.NotFound(); query = query.Where(x => x.FarmId == farmId); } else if (!resolved.IsUnrestricted) query = query.Where(x => resolved.FarmIds!.Contains(x.FarmId)); AnalyticsFact[] facts = await query.ToArrayAsync(ct); return UseCaseResults.Ok(facts.GroupBy(x => new { x.Crop, x.Variety, x.Unit, x.ValueKind }).Select(x => new { x.Key.Crop, x.Key.Variety, x.Key.Unit, x.Key.ValueKind, average = decimal.Round(x.Average(f => f.Value), 4), minimum = x.Min(f => f.Value), maximum = x.Max(f => f.Value), dataQualityPercent = decimal.Round(x.Average(f => f.DataQualityPercent), 2), observations = x.Count() }).OrderByDescending(x => x.average)); }


}
