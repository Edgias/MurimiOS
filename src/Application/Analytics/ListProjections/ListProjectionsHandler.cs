using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Analytics.ListProjections;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using AnalyticsProjectionStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionStatus;

public sealed class ListProjectionsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListProjectionsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListProjectionsQuery message, CancellationToken cancellationToken) => ListProjectionsAsync(message.farmId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListProjectionsAsync(Guid farmId, AnalyticsProjectionStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    { if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound(); IQueryable<AnalyticsProjectionRun> query = db.AnalyticsProjectionRuns.AsNoTracking().Where(x => x.FarmId == farmId); if (status.HasValue) query = query.Where(x => x.Status == status); return UseCaseResults.Ok(await query.OrderByDescending(x => x.ScheduledFor).ToArrayAsync(ct)); }


}
