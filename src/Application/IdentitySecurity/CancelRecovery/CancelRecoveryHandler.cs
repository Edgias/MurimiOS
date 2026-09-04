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
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.CancelRecovery;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;

public sealed class CancelRecoveryHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor) : IRequestHandler<CancelRecoveryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CancelRecoveryCommand message, CancellationToken cancellationToken) => CancelRecoveryAsync(message.requestId, db, routing, actor, cancellationToken);

    private static async Task<UseCaseResult> CancelRecoveryAsync(Guid requestId, IPlatformDbContext db, TenantDatabaseRoutingScope routing,
        IRequestActorContext actor, CancellationToken ct)
    { using IDisposable control = routing.EnterControlPlane(); AccountRecoveryRequest? request = await db.AccountRecoveryRequests.SingleOrDefaultAsync(x => x.Id == requestId, ct); if (request is null) return UseCaseResults.NotFound(); try { request.Cancel(DateTimeOffset.UtcNow, actor.Subject); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(new { request.Id, request.Status, request.ClosedAt, request.ClosedBy }); } catch (InvalidOperationException e) { return Conflict("recovery_transition_invalid", e.Message); } }

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
