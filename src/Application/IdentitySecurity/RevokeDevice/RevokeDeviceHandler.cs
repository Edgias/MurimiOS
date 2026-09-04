using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IdentityDevice;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.IdentitySecurity.RevokeDevice;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using IdentitySessionStatus = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySessionStatus;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;

public sealed class RevokeDeviceHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing, IRequestActorContext actor, UseCaseContext context) : IRequestHandler<RevokeDeviceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(RevokeDeviceCommand message, CancellationToken cancellationToken) => RevokeDeviceAsync(message.deviceId, message.request, db, routing, actor, context, cancellationToken);

    private static async Task<UseCaseResult> RevokeDeviceAsync(Guid deviceId, SecurityReasonRequest request, IPlatformDbContext db,
        TenantDatabaseRoutingScope routing, IRequestActorContext actor, UseCaseContext context, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); IdentityDevice? device = await db.IdentityDevices.SingleOrDefaultAsync(x => x.Id == deviceId, ct); if (device is null) return UseCaseResults.NotFound();
        try { DateTimeOffset now = DateTimeOffset.UtcNow; device.Revoke(now, actor.Subject, request.Reason); IdentitySession[] sessions = await db.IdentitySessions.Where(x => x.DeviceId == device.Id && x.Status == IdentitySessionStatus.Active).ToArrayAsync(ct); foreach (IdentitySession session in sessions) session.Revoke(now, actor.Subject, "Device revoked: " + request.Reason); db.SecurityEvents.Add(Event("DeviceRevoked", actor.Subject, null, device.Id, context, new { request.Reason, sessionsRevoked = sessions.Length }, now)); await db.SaveChangesAsync(ct); return UseCaseResults.Ok(new { device, sessionsRevoked = sessions.Length }); }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException) { return Conflict("device_revocation_invalid", e.Message); }
    }


    private static SecurityEvent Event(string type, string? subject, Guid? sessionId, Guid? deviceId, UseCaseContext context,
        object details, DateTimeOffset now, SecurityEventOutcome outcome = SecurityEventOutcome.Succeeded) => new(Guid.NewGuid(), type, outcome, subject, sessionId, deviceId, now, context.User.CorrelationId, HashOptional(context.User.RemoteAddress?.ToString()), HashOptional(context.User.Header("User-Agent")), JsonSerializer.Serialize(details));

    private static string? HashOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : Sha(value);

    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static string Sha(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

}
