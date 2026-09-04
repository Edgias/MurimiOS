using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.FarmDashboard;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;

public sealed class FarmDashboardHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<FarmDashboardQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(FarmDashboardQuery message, CancellationToken cancellationToken) => FarmDashboardAsync(message.farmId, message.periodEnd, db, scope, cancellationToken);

    private static async Task<UseCaseResult> FarmDashboardAsync(Guid farmId, DateOnly? periodEnd, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    { if (!await scope.CanAccessFarmAsync(farmId, ct) || !await db.Farms.AnyAsync(x => x.Id == farmId, ct)) return UseCaseResults.NotFound(); IQueryable<AnalyticsFact> current = global::EdgIQ.MurimiOS.Application.Common.Analytics.AnalyticsFacts.CurrentFacts(db); DateOnly end = periodEnd ?? await current.Where(x => x.FarmId == farmId).MaxAsync(x => (DateOnly?)x.PeriodEnd, ct) ?? DateOnly.FromDateTime(DateTime.UtcNow); AnalyticsFact[] facts = await current.Where(x => x.FarmId == farmId && x.PeriodEnd == end).ToArrayAsync(ct); return UseCaseResults.Ok(Dashboard(facts, new { farmId, periodEnd = end })); }

    private static object Dashboard(AnalyticsFact[] facts, object scope) => new { scope, metrics = facts.GroupBy(x => new { x.MetricCode, x.Unit, x.ValueKind }).Select(x => new { x.Key.MetricCode, x.Key.Unit, x.Key.ValueKind, value = x.Sum(f => f.Value), average = x.Average(f => f.Value), dataQualityPercent = decimal.Round(x.Average(f => f.DataQualityPercent), 2), grains = x.Count() }), exceptions = facts.Where(x => x.DataQualityPercent < 60).Select(x => new { x.MetricCode, x.GrainKey, x.DataQualityPercent }).Take(100) };


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
