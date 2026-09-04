using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListDimensions;
using AnalyticsDimensionSnapshot = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshot;
using AnalyticsDimensionType = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionType;

public sealed class ListDimensionsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListDimensionsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListDimensionsQuery message, CancellationToken cancellationToken) => ListDimensionsAsync(message.farmId, message.projectionRunId, message.type, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListDimensionsAsync(Guid farmId, Guid? projectionRunId,
        AnalyticsDimensionType? type, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        IQueryable<AnalyticsDimensionSnapshot> query = db.AnalyticsDimensionSnapshots.AsNoTracking().Where(x => x.FarmId == farmId);
        if (projectionRunId is { } runId) query = query.Where(x => x.ProjectionRunId == runId);
        if (type is { } value) query = query.Where(x => x.DimensionType == value);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.ValidOn).ThenBy(x => x.GrainKey).Take(5_000).ToArrayAsync(ct));
    }


}
