using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SensorReading;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.QueryReadings;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;

public sealed class QueryReadingsHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<QueryReadingsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(QueryReadingsQuery message, CancellationToken cancellationToken) => QueryReadingsAsync(message.farmId, message.deviceId, message.metricCode, message.from, message.to, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> QueryReadingsAsync(Guid farmId, Guid? deviceId, string? metricCode,
        DateTimeOffset? from, DateTimeOffset? to, int? limit, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        DateTimeOffset end = to ?? DateTimeOffset.UtcNow; DateTimeOffset start = from ?? end.AddDays(-7); int take = Math.Clamp(limit ?? 1_000, 1, 10_000);
        IQueryable<SensorReading> query = db.SensorReadings.AsNoTracking().Where(x => x.FarmId == farmId && x.ObservedAt >= start && x.ObservedAt < end);
        if (deviceId.HasValue) query = query.Where(x => x.DeviceId == deviceId);
        if (!string.IsNullOrWhiteSpace(metricCode)) query = query.Where(x => x.MetricCode == metricCode.Trim().ToLower());
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.ObservedAt).Take(take).ToArrayAsync(ct));
    }


}
