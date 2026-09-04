using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ConnectedAgriculture.QueryTelemetry;
using MachineryTelemetryRecord = global::EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord.MachineryTelemetryRecord;

public sealed class QueryTelemetryHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<QueryTelemetryQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(QueryTelemetryQuery message, CancellationToken cancellationToken) => QueryTelemetryAsync(message.farmId, message.equipmentId, message.from, message.to, message.limit, db, scope, cancellationToken);

    private static async Task<UseCaseResult> QueryTelemetryAsync(Guid farmId, Guid? equipmentId, DateTimeOffset? from,
        DateTimeOffset? to, int? limit, IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        DateTimeOffset end = to ?? DateTimeOffset.UtcNow; DateTimeOffset start = from ?? end.AddDays(-7); int take = Math.Clamp(limit ?? 1_000, 1, 10_000);
        IQueryable<MachineryTelemetryRecord> query = db.MachineryTelemetry.AsNoTracking().Where(x => x.FarmId == farmId && x.ObservedAt >= start && x.ObservedAt < end);
        if (equipmentId.HasValue) query = query.Where(x => x.EquipmentId == equipmentId);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.ObservedAt).Take(take).ToArrayAsync(ct));
    }


}
