using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Infrastructure.Access;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IdentityDevice;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using EdgIQ.MurimiOS.Domain.SecurityEvent;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Infrastructure.Tenancy;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentityDeviceStatus = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDeviceStatus;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using IdentitySessionStatus = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySessionStatus;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;
using IPlatformDbContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IPlatformDbContext;
using IRequestActorContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IRequestActorContext;
using TenantDatabaseRoutingScope = global::EdgIQ.MurimiOS.Application.Common.Abstractions.TenantDatabaseRoutingScope;
using DevelopmentHeaderAuthenticationHandler = global::EdgIQ.MurimiOS.API.Infrastructure.Access.DevelopmentHeaderAuthenticationHandler;

public sealed class IdentitySecurityOptions
{
    public bool RequireMfaForWrites { get; set; } = true;
    public string MfaClaimType { get; set; } = "amr";
    public string[] MfaAcceptedValues { get; set; } = ["mfa", "otp", "hwk"];
    public bool AllowDevelopmentBypass { get; set; } = true;
    public int SessionTouchMinutes { get; set; } = 5;
    public void Validate(IWebHostEnvironment environment, IConfiguration configuration)
    {
        if (SessionTouchMinutes is < 1 or > 60 || string.IsNullOrWhiteSpace(MfaClaimType) || MfaAcceptedValues.Length == 0)
            throw new InvalidOperationException("Authentication session and MFA configuration is invalid.");
        if (environment.IsDevelopment()) return;
        string authority = configuration["Authentication:Authority"] ?? "";
        string audience = configuration["Authentication:Audience"] ?? "";
        if (!Uri.TryCreate(authority, UriKind.Absolute, out Uri? uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("Production Authentication:Authority must be an absolute HTTPS identity-provider URL.");
        if (string.IsNullOrWhiteSpace(audience)) throw new InvalidOperationException("Production Authentication:Audience is required.");
        if (!configuration.GetValue("Authentication:RequireHttpsMetadata", true))
            throw new InvalidOperationException("Production identity-provider metadata must require HTTPS.");
    }
}

public sealed class IdentitySecurityMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IPlatformDbContext db, TenantDatabaseRoutingScope routing,
        IRequestActorContext actor, IdentitySecurityOptions options, IWebHostEnvironment environment, TimeProvider timeProvider)
    {
        if (!actor.IsResolved || context.Request.Path.StartsWithSegments("/api/public/v1"))
        { await next(context); return; }

        bool developmentBypass = environment.IsDevelopment() && options.AllowDevelopmentBypass
            && context.User.HasClaim("auth_method", DevelopmentHeaderAuthenticationHandler.AuthenticationMethod);
        bool mfa = developmentBypass || HasMfa(context.User, options);
        DateTimeOffset now = timeProvider.GetUtcNow();
        if (options.RequireMfaForWrites && !IsRead(context.Request.Method) && !mfa)
        {
            await RecordEventAsync(db, routing, actor.Subject, null, null, "MfaRequired", SecurityEventOutcome.Rejected,
                context, new { path = context.Request.Path.Value, method = context.Request.Method }, now);
            await Results.Json(new { code = "mfa_required", message = "Multi-factor authentication is required for write operations." }, statusCode: StatusCodes.Status403Forbidden).ExecuteAsync(context);
            return;
        }

        string? rawSession = context.User.FindFirst("sid")?.Value ?? context.User.FindFirst("jti")?.Value;
        if (string.IsNullOrWhiteSpace(rawSession)) { await next(context); return; }
        if (!TryUnixClaim(context.User, "exp", out DateTimeOffset expiresAt) || expiresAt <= now)
        {
            await Results.Json(new { code = "session_expired", message = "The authenticated session has expired or lacks a valid expiry." }, statusCode: StatusCodes.Status401Unauthorized).ExecuteAsync(context);
            return;
        }
        DateTimeOffset authenticatedAt = TryUnixClaim(context.User, "auth_time", out DateTimeOffset issued) ? issued : now;
        string sessionHash = Hash(rawSession);
        string? rawDevice = context.User.FindFirst("device_id")?.Value ?? context.Request.Headers["X-Device-Id"].FirstOrDefault();
        string? deviceHash = string.IsNullOrWhiteSpace(rawDevice) ? null : Hash(rawDevice);
        bool rejected = false; string rejectionCode = ""; IdentitySession? session = null; IdentityDevice? device = null;

        using (routing.EnterControlPlane())
        {
            if (deviceHash is not null)
            {
                device = await db.IdentityDevices.SingleOrDefaultAsync(x => x.DeviceKeyHash == deviceHash, context.RequestAborted);
                if (device is null)
                {
                    device = new IdentityDevice(Guid.NewGuid(), deviceHash, actor.Subject,
                        context.Request.Headers["X-Device-Name"].FirstOrDefault() ?? "Unlabelled device",
                        context.Request.Headers["X-Device-Platform"].FirstOrDefault() ?? "unknown", now);
                    db.IdentityDevices.Add(device);
                }
                else if (device.Status == IdentityDeviceStatus.Revoked) { rejected = true; rejectionCode = "device_revoked"; }
            }
            session = await db.IdentitySessions.SingleOrDefaultAsync(x => x.SessionKeyHash == sessionHash, context.RequestAborted);
            if (session?.Status != IdentitySessionStatus.Active) { if (session is not null) { rejected = true; rejectionCode = "session_revoked"; } }
            if (session is not null && !session.Subject.Equals(actor.Subject, StringComparison.Ordinal)) { rejected = true; rejectionCode = "session_subject_mismatch"; }
            if (session is not null && device is not null && session.DeviceId != device.Id) { rejected = true; rejectionCode = "session_device_mismatch"; }
            if (session is null && !rejected)
            {
                session = new IdentitySession(Guid.NewGuid(), sessionHash, actor.Subject, device?.Id, authenticatedAt,
                    expiresAt, mfa, context.User.FindFirst("auth_method")?.Value ?? "oidc", RemoteHash(context), UserAgentHash(context));
                db.IdentitySessions.Add(session);
                db.SecurityEvents.Add(Event("SessionStarted", SecurityEventOutcome.Succeeded, actor.Subject, session.Id, device?.Id, context, new { mfa }, now));
            }
            else if (session is not null && !rejected)
            {
                if (session.ExpiresAt <= now) { session.Expire(now); rejected = true; rejectionCode = "session_expired"; }
                else if (now - session.LastSeenAt >= TimeSpan.FromMinutes(options.SessionTouchMinutes)) session.Seen(now);
            }
            if (device is not null && !rejected && now - device.LastSeenAt >= TimeSpan.FromMinutes(options.SessionTouchMinutes)) device.Seen(now);
            if (rejected) db.SecurityEvents.Add(Event("SessionRejected", SecurityEventOutcome.Rejected, actor.Subject, session?.Id, device?.Id, context, new { code = rejectionCode }, now));
            if (db.ChangeTracker.HasChanges()) await db.SaveChangesAsync(context.RequestAborted);
        }
        if (rejected)
        {
            await Results.Json(new { code = rejectionCode, message = "The authenticated device or session is no longer permitted." }, statusCode: StatusCodes.Status401Unauthorized).ExecuteAsync(context);
            return;
        }
        await next(context);
    }

    private static bool HasMfa(ClaimsPrincipal principal, IdentitySecurityOptions options) => principal.FindAll(options.MfaClaimType)
        .SelectMany(x => x.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        .Any(value => options.MfaAcceptedValues.Contains(value, StringComparer.OrdinalIgnoreCase));
    private static bool IsRead(string method) => HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method);
    private static bool TryUnixClaim(ClaimsPrincipal principal, string type, out DateTimeOffset value)
    { value = default; return long.TryParse(principal.FindFirst(type)?.Value, NumberStyles.None, CultureInfo.InvariantCulture, out long seconds) && TryUnix(seconds, out value); }
    private static bool TryUnix(long seconds, out DateTimeOffset value) { try { value = DateTimeOffset.FromUnixTimeSeconds(seconds); return true; } catch (ArgumentOutOfRangeException) { value = default; return false; } }
    private static string Hash(string value) => Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    private static string? RemoteHash(HttpContext context) => context.Connection.RemoteIpAddress is { } address ? Hash(address.ToString()) : null;
    private static string? UserAgentHash(HttpContext context) { string value = context.Request.Headers.UserAgent.ToString(); return string.IsNullOrWhiteSpace(value) ? null : Hash(value); }
    private static SecurityEvent Event(string type, SecurityEventOutcome outcome, string? subject, Guid? sessionId,
        Guid? deviceId, HttpContext context, object details, DateTimeOffset now) => new(Guid.NewGuid(), type, outcome,
            subject, sessionId, deviceId, now, context.TraceIdentifier, RemoteHash(context), UserAgentHash(context), JsonSerializer.Serialize(details));
    private static async Task RecordEventAsync(IPlatformDbContext db, TenantDatabaseRoutingScope routing, string? subject,
        Guid? sessionId, Guid? deviceId, string type, SecurityEventOutcome outcome, HttpContext context, object details, DateTimeOffset now)
    { using IDisposable control = routing.EnterControlPlane(); db.SecurityEvents.Add(Event(type, outcome, subject, sessionId, deviceId, context, details, now)); await db.SaveChangesAsync(context.RequestAborted); }
}
