using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.RevokeSession;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;

public sealed class RevokeSessionHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<RevokeSessionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RevokeSessionCommand message, CancellationToken cancellationToken) => RevokeSessionAsync(message.sessionId, message.request, db, routing, actor, context, cancellationToken);

    private static async Task<UseCaseResult> RevokeSessionAsync(Guid sessionId, SecurityReasonRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); IdentitySession? session = await db.IdentitySessions.SingleOrDefaultAsync(x => x.Id == sessionId, ct); if (session is null) return UseCaseResults.NotFound();
        try { DateTimeOffset now = DateTimeOffset.UtcNow; session.Revoke(now, actor.Subject, request.Reason); db.SecurityEvents.Add(Event("SessionRevoked", actor.Subject, session.Id, session.DeviceId, context, new { request.Reason }, now)); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(session); }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Conflict("session_revocation_invalid", e.Message); }
    }


    private static SecurityEvent Event(string type, string? subject, Guid? sessionId, Guid? deviceId, UseCaseContext context,
        object details, DateTimeOffset now, SecurityEventOutcome outcome = SecurityEventOutcome.Succeeded) => new(Guid.NewGuid(), type, outcome, subject, sessionId, deviceId, now, context.User.CorrelationId, HashOptional(context.User.RemoteAddress?.ToString()), HashOptional(context.User.Header("User-Agent")), JsonSerializer.Serialize(details));

    private static string? HashOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : Sha(value);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static string Sha(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

}
