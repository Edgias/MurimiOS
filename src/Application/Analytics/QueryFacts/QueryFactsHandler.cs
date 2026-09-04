using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.QueryFacts;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsValueKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsValueKind;

public sealed class QueryFactsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<QueryFactsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(QueryFactsQuery message, CancellationToken cancellationToken) => QueryFactsAsync(message.farmId, message.metricCode, message.valueKind, message.from, message.to, db, scope, cancellationToken);

    private static async Task<UseCaseResult> QueryFactsAsync(Guid farmId, string? metricCode, AnalyticsValueKind? valueKind, DateOnly? from, DateOnly? to, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    { if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound(); IQueryable<AnalyticsFact> query = global::EdgIQ.MurimiOS.Application.Common.Analytics.AnalyticsFacts.CurrentFacts(db).Where(x => x.FarmId == farmId); if (!string.IsNullOrWhiteSpace(metricCode)) query = query.Where(x => x.MetricCode == metricCode.Trim().ToLower()); if (valueKind.HasValue) query = query.Where(x => x.ValueKind == valueKind); if (from.HasValue) query = query.Where(x => x.PeriodEnd >= from); if (to.HasValue) query = query.Where(x => x.PeriodStart <= to); return UseCaseResults.Ok(await query.OrderByDescending(x => x.PeriodEnd).ThenBy(x => x.MetricCode).Take(10_000).ToArrayAsync(ct)); }


}
