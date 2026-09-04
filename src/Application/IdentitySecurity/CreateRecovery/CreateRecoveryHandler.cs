using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.CreateRecovery;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;
using AccountRecoveryStatus = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

public sealed class CreateRecoveryHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor, ITenantContext tenant, UseCaseContext context) : IRequestHandler<CreateRecoveryCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateRecoveryCommand message, CancellationToken cancellationToken) => CreateRecoveryAsync(message.request, db, routing, actor, tenant, context, cancellationToken);

    private static async Task<UseCaseResult> CreateRecoveryAsync(CreateRecoveryRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, ITenantContext tenant, UseCaseContext context, CancellationToken ct)
    {
        TenantMembership? membership = await db.TenantMemberships.AsNoTracking().SingleOrDefaultAsync(x => x.Id == request.MembershipId, ct); if (membership is null) return UseCaseResults.NotFound();
        DateTimeOffset now = DateTimeOffset.UtcNow; DateTimeOffset expires = request.ExpiresAt ?? now.AddMinutes(30);
        if (expires <= now || expires > now.AddHours(24)) return Bad("recovery_expiry_invalid", "Recovery must expire within 24 hours.");
        (string secret, string prefix, string hash) = Secret();
        using IDisposable control = routing.EnterControlPlane(); AccountRecoveryRequest[] pending = await db.AccountRecoveryRequests.Where(x => x.MembershipId == membership.Id && x.Status == AccountRecoveryStatus.Pending).ToArrayAsync(ct); foreach (AccountRecoveryRequest old in pending) old.Cancel(now, actor.Subject);
        try { var recovery = new AccountRecoveryRequest(Id(request.Id), membership.Id, membership.ExternalSubject, prefix, hash, now, expires, request.MaximumAttempts, actor.Subject); db.AccountRecoveryRequests.Add(recovery); db.SecurityEvents.Add(Event("RecoveryIssued", actor.Subject, null, null, context, new { recoveryId = recovery.Id, membershipId = membership.Id }, now)); db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "Identity.RecoveryIssued", JsonSerializer.Serialize(new { recoveryId = recovery.Id, membershipId = membership.Id, recovery.TokenPrefix, recovery.ExpiresAt }), now)); await db.SaveChangesAsync(ct); return UseCaseResults.Created($"/api/v1/security/recovery-requests/{recovery.Id}", new { recovery = new { recovery.Id, recovery.MembershipId, recovery.Subject, recovery.TokenPrefix, recovery.Status, recovery.RequestedAt, recovery.ExpiresAt, recovery.MaximumAttempts }, secret }); }
        catch (ArgumentException e) { return Bad("recovery_request_invalid", e.Message); }
    }


    private static SecurityEvent Event(string type, string? subject, Guid? sessionId, Guid? deviceId, UseCaseContext context,
        object details, DateTimeOffset now, SecurityEventOutcome outcome = SecurityEventOutcome.Succeeded) => new(Guid.NewGuid(), type, outcome, subject, sessionId, deviceId, now, context.User.CorrelationId, HashOptional(context.User.RemoteAddress?.ToString()), HashOptional(context.User.Header("User-Agent")), JsonSerializer.Serialize(details));

    private static (string Secret, string Prefix, string Hash) Secret() { string raw = "mro_recovery_" + EdgIQ.MurimiOS.Application.Common.TextEncoding.Base64UrlEncoding.Encode(RandomNumberGenerator.GetBytes(32)); return (raw, raw[..Math.Min(24, raw.Length)], Sha(raw)); }

    private static string Sha(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static string? HashOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : Sha(value);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
