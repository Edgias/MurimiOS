using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.ListRecovery;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;
using AccountRecoveryStatus = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryStatus;

public sealed class ListRecoveryHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ListRecoveryQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRecoveryQuery message, CancellationToken cancellationToken) => ListRecoveryAsync(message.status, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ListRecoveryAsync(AccountRecoveryStatus? status, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); IQueryable<AccountRecoveryRequest> q = db.AccountRecoveryRequests.AsNoTracking(); if (status.HasValue) q = q.Where(x => x.Status == status); return UseCaseResults.Ok(await q.OrderByDescending(x => x.RequestedAt).Take(5_000).Select(x => new { x.Id, x.MembershipId, x.Subject, x.TokenPrefix, x.Status, x.RequestedAt, x.ExpiresAt, x.MaximumAttempts, x.FailedAttempts, x.RequestedBy, x.VerifiedAt, x.ClosedAt, x.ClosedBy }).ToArrayAsync(ct)); }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

}
