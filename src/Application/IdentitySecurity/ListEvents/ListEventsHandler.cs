using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListEvents;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;

public sealed class ListEventsHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListEventsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListEventsQuery message, CancellationToken cancellationToken) => ListEventsAsync(message.eventType, message.outcome, message.from, message.to, message.limit, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListEventsAsync(string? eventType, SecurityEventOutcome? outcome, DateTimeOffset? from,
        DateTimeOffset? to, int? limit, IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); IQueryable<SecurityEvent> q = db.SecurityEvents.AsNoTracking(); if (!string.IsNullOrWhiteSpace(eventType)) q = q.Where(x => x.EventType == eventType); if (outcome.HasValue) q = q.Where(x => x.Outcome == outcome); if (from.HasValue) q = q.Where(x => x.OccurredAt >= from); if (to.HasValue) q = q.Where(x => x.OccurredAt <= to); return UseCaseResults.Ok(await q.OrderByDescending(x => x.OccurredAt).Take(Math.Clamp(limit ?? 500, 1, 10_000)).ToArrayAsync(ct)); }


}
