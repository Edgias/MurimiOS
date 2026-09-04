using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListSessions;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using IdentitySessionStatus = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySessionStatus;

public sealed class ListSessionsHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListSessionsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListSessionsQuery message, CancellationToken cancellationToken) => ListSessionsAsync(message.subject, message.status, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListSessionsAsync(string? subject, IdentitySessionStatus? status, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); IQueryable<IdentitySession> q = db.IdentitySessions.AsNoTracking(); if (!string.IsNullOrWhiteSpace(subject)) q = q.Where(x => x.Subject == subject); if (status.HasValue) q = q.Where(x => x.Status == status); return UseCaseResults.Ok(await q.OrderByDescending(x => x.LastSeenAt).Take(10_000).ToArrayAsync(ct)); }


}
