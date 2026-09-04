using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.VerifyRecovery;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using IdentitySessionStatus = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySessionStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;

public sealed class VerifyRecoveryHandler(IMutableTenantContext tenant, IPlatformDbContext db, TenantDatabaseRoutingScope routing, UseCaseContext context) : IRequestHandler<VerifyRecoveryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(VerifyRecoveryCommand message, CancellationToken cancellationToken) => VerifyRecoveryAsync(message.request, tenant, db, routing, context, cancellationToken);

    private static async Task<UseCaseResult> VerifyRecoveryAsync(VerifyRecoveryRequest request, IMutableTenantContext tenant,
        IPlatformDbContext db, TenantDatabaseRoutingScope routing, UseCaseContext context, CancellationToken ct)
    {
        if (request.TenantId == Guid.Empty || request.RequestId == Guid.Empty || string.IsNullOrWhiteSpace(request.Token)) return Bad("recovery_verification_invalid", "Tenant, request, and token are required.");
        tenant.Resolve(request.TenantId); DateTimeOffset now = DateTimeOffset.UtcNow; string hash = Sha(request.Token);
        using IDisposable control = routing.EnterControlPlane(); AccountRecoveryRequest? recovery = await db.AccountRecoveryRequests.SingleOrDefaultAsync(x => x.Id == request.RequestId, ct); if (recovery is null) return Bad("recovery_verification_invalid", "Recovery request or token is invalid.");
        bool matches = CryptographicOperations.FixedTimeEquals(Convert.FromHexString(hash), Convert.FromHexString(recovery.TokenHash));
        try
        {
            if (!matches) { recovery.RecordFailure(now); db.SecurityEvents.Add(Event("RecoveryRejected", recovery.Subject, null, null, context, new { recovery.Id }, now, SecurityEventOutcome.Rejected)); await db.SaveChangesAsync(ct); return Bad("recovery_verification_invalid", "Recovery request or token is invalid."); }
            recovery.Verify(now); IdentitySession[] sessions = await db.IdentitySessions.Where(x => x.Subject == recovery.Subject && x.Status == IdentitySessionStatus.Active).ToArrayAsync(ct); foreach (IdentitySession session in sessions) session.Revoke(now, "account-recovery", "Verified account recovery revoked existing sessions."); db.SecurityEvents.Add(Event("RecoveryVerified", recovery.Subject, null, null, context, new { recovery.Id, sessionsRevoked = sessions.Length }, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), request.TenantId, "Identity.RecoveryVerified", JsonSerializer.Serialize(new { recovery.Id, recovery.MembershipId, sessionsRevoked = sessions.Length }), now)); await db.SaveChangesAsync(ct); return UseCaseResults.Accepted(value: new { recovery.Id, recovery.Status, sessionsRevoked = sessions.Length, externalIdentityProviderActionRequired = true });
        }
        catch (InvalidOperationException e) { await db.SaveChangesAsync(ct); return Conflict("recovery_transition_invalid", e.Message); }
    }


    private static SecurityEvent Event(string type, string? subject, Guid? sessionId, Guid? deviceId, UseCaseContext context,
        object details, DateTimeOffset now, SecurityEventOutcome outcome = SecurityEventOutcome.Succeeded) => new(Guid.NewGuid(), type, outcome, subject, sessionId, deviceId, now, context.User.CorrelationId, HashOptional(context.User.RemoteAddress?.ToString()), HashOptional(context.User.Header("User-Agent")), JsonSerializer.Serialize(details));

    private static string Sha(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static string? HashOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : Sha(value);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
