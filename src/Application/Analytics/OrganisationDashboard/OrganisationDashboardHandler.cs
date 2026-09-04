using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.OrganisationDashboard;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;

public sealed class OrganisationDashboardHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<OrganisationDashboardQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(OrganisationDashboardQuery message, CancellationToken cancellationToken) => OrganisationDashboardAsync(message.organisationNodeId, message.periodEnd, db, scope, cancellationToken);

    private static async Task<UseCaseResult> OrganisationDashboardAsync(Guid organisationNodeId, DateOnly? periodEnd, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    { ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct); if (!resolved.AllowsNode(organisationNodeId)) return UseCaseResults.NotFound(); string? rootPath = await db.OrganisationNodes.Where(x => x.Id == organisationNodeId).Select(x => x.Path).SingleOrDefaultAsync(ct); if (rootPath is null) return UseCaseResults.NotFound(); Guid[] nodeIds; if (db.Database.ProviderName?.Contains("Npgsql", StringComparison.Ordinal) == true) nodeIds = await db.OrganisationNodes.FromSqlInterpolated($"select node.* from platform.org_nodes node where node.path <@ CAST({rootPath} AS ltree)").Select(x => x.Id).ToArrayAsync(ct); else nodeIds = await db.OrganisationNodes.Where(x => x.Path == rootPath || x.Path.StartsWith(rootPath + ".")).Select(x => x.Id).ToArrayAsync(ct); Guid[] farmIds = await db.Farms.Where(x => nodeIds.Contains(x.OrganisationNodeId)).Select(x => x.Id).ToArrayAsync(ct); if (!resolved.IsUnrestricted) farmIds = farmIds.Where(resolved.AllowsFarm).ToArray(); IQueryable<AnalyticsFact> current = global::EdgIQ.MurimiOS.Application.Common.Analytics.AnalyticsFacts.CurrentFacts(db); DateOnly end = periodEnd ?? await current.Where(x => farmIds.Contains(x.FarmId)).MaxAsync(x => (DateOnly?)x.PeriodEnd, ct) ?? DateOnly.FromDateTime(DateTime.UtcNow); AnalyticsFact[] facts = await current.Where(x => farmIds.Contains(x.FarmId) && x.PeriodEnd == end).ToArrayAsync(ct); return UseCaseResults.Ok(Dashboard(facts, new { organisationNodeId, periodEnd = end, farmCount = farmIds.Length })); }

    private static object Dashboard(AnalyticsFact[] facts, object scope) => new { scope, metrics = facts.GroupBy(x => new { x.MetricCode, x.Unit, x.ValueKind }).Select(x => new { x.Key.MetricCode, x.Key.Unit, x.Key.ValueKind, value = x.Sum(f => f.Value), average = x.Average(f => f.Value), dataQualityPercent = decimal.Round(x.Average(f => f.DataQualityPercent), 2), grains = x.Count() }), exceptions = facts.Where(x => x.DataQualityPercent < 60).Select(x => new { x.MetricCode, x.GrainKey, x.DataQualityPercent }).Take(100) };


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
