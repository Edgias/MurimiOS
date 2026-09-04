using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Satellite;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;

public sealed class SatelliteHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<SatelliteQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(SatelliteQuery message, CancellationToken cancellationToken) => SatelliteAsync(message.fieldId, message.index, message.from, message.to, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> SatelliteAsync(Guid fieldId, VegetationIndex? index, DateTimeOffset? from,
        DateTimeOffset? to, int? limit, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        if (!await FieldAllowedAsync(fieldId, db, scope, cancellationToken)) return UseCaseResults.NotFound();
        IQueryable<SatelliteFieldStatistic> query = db.SatelliteFieldStatistics.AsNoTracking().Where(x => x.FieldId == fieldId);
        if (index is { } selected) query = query.Where(x => x.Index == selected);
        if (from is { } start) query = query.Where(x => x.AcquiredAt >= start.ToUniversalTime());
        if (to is { } end) query = query.Where(x => x.AcquiredAt <= end.ToUniversalTime());
        SatelliteFieldStatistic[] rows = await query.OrderByDescending(x => x.AcquiredAt).Take(Math.Clamp(limit ?? 250, 1, 2_000)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows.Select(ToSatellite).ToArray());
    }


    private static async Task<bool> FieldAllowedAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        Guid? farmId = await db.FarmFields.AsNoTracking().Where(x => x.Id == fieldId).Select(x => (Guid?)x.FarmId).SingleOrDefaultAsync(cancellationToken);
        return farmId is { } farm && await scope.CanAccessFarmAsync(farm, cancellationToken);
    }

    private static SatelliteFieldStatisticResponse ToSatellite(SatelliteFieldStatistic x) => new(x.Id, x.FarmId,
        x.FieldId, x.BoundaryVersionId, x.FeedId, x.IngestionRecordId, x.ProviderAcquisitionId, x.Index,
        x.CustomIndex, x.AcquiredAt, x.ProcessedAt, x.Mean, x.Minimum, x.Maximum, x.StandardDeviation,
        x.ValidPixelPercent, x.CloudPercent, x.Quality);

}
