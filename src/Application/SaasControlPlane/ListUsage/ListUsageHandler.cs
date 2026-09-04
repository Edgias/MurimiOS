using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ListUsage;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;

using UsageMeterEvent = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterEvent;

public sealed class ListUsageHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListUsageQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListUsageQuery message, CancellationToken cancellationToken) => ListUsageAsync(message.metricCode, message.from, message.to, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListUsageAsync(string? metricCode, DateTimeOffset? from, DateTimeOffset? to,
        IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); IQueryable<UsageMeterEvent> query = db.UsageMeterEvents.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(metricCode)) query = query.Where(x => x.MetricCode == metricCode.Trim().ToLower());
        if (from.HasValue) query = query.Where(x => x.OccurredAt >= from.Value); if (to.HasValue) query = query.Where(x => x.OccurredAt <= to.Value);
        return UseCaseResults.Ok(await query.OrderByDescending(x => x.OccurredAt).Take(10_000).ToArrayAsync(ct));
    }


}
