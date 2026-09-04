using EdgIQ.MurimiOS.Domain.IdentityDevice;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy;
using EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;
using AccountRecoveryStatus = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryStatus;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentityDeviceStatus = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDeviceStatus;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using IdentitySessionStatus = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySessionStatus;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class IdentitySecurityTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-09-01T10:00:00Z");
    private const string Hash = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    [Fact]
    public void Quota_and_rotation_policies_validate_limits_and_have_explicit_retirement()
    {
        var quota = new ApiRateLimitPolicy(Guid.NewGuid(), null, 100, 60, 10_000, Now, "security-admin");
        var rotation = new ApiCredentialRotationPolicy(Guid.NewGuid(), null, 90, 14, true, Now, "security-admin");

        quota.Retire(Now.AddDays(1));
        rotation.Retire(Now.AddDays(1));

        Assert.False(quota.IsActive);
        Assert.False(rotation.IsActive);
        Assert.Throws<InvalidOperationException>(() => quota.Retire(Now.AddDays(2)));
        Assert.Throws<ArgumentException>(() =>
            new ApiRateLimitPolicy(Guid.NewGuid(), null, 101, 60, 100, Now, "security-admin"));
        Assert.Throws<ArgumentException>(() =>
            new ApiCredentialRotationPolicy(Guid.NewGuid(), null, 30, 30, true, Now, "security-admin"));
    }

    [Fact]
    public void Revoking_a_device_or_session_is_terminal()
    {
        var device = new IdentityDevice(Guid.NewGuid(), Hash, "user-1", "Field tablet", "android", Now);
        var session = new IdentitySession(Guid.NewGuid(), Hash, "user-1", device.Id, Now, Now.AddHours(8),
            true, "mfa", null, null);

        device.Revoke(Now.AddMinutes(5), "security-admin", "Device reported lost");
        session.Revoke(Now.AddMinutes(5), "security-admin", "Device reported lost");

        Assert.Equal(IdentityDeviceStatus.Revoked, device.Status);
        Assert.Equal(IdentitySessionStatus.Revoked, session.Status);
        Assert.Throws<InvalidOperationException>(() => device.Seen(Now.AddMinutes(6)));
        Assert.Throws<InvalidOperationException>(() => session.Seen(Now.AddMinutes(6)));
    }

    [Fact]
    public void Recovery_attempts_lock_the_token_and_success_has_a_terminal_state()
    {
        var locked = new AccountRecoveryRequest(Guid.NewGuid(), Guid.NewGuid(), "user-1", "mro_recovery_test",
            Hash, Now, Now.AddMinutes(30), 2, "security-admin");
        locked.RecordFailure(Now.AddMinutes(1));
        locked.RecordFailure(Now.AddMinutes(2));
        Assert.Equal(AccountRecoveryStatus.Locked, locked.Status);
        Assert.Throws<InvalidOperationException>(() => locked.Verify(Now.AddMinutes(3)));

        var verified = new AccountRecoveryRequest(Guid.NewGuid(), Guid.NewGuid(), "user-2", "mro_recovery_test",
            Hash, Now, Now.AddMinutes(30), 5, "security-admin");
        verified.Verify(Now.AddMinutes(1));
        Assert.Equal(AccountRecoveryStatus.Verified, verified.Status);
        Assert.NotNull(verified.VerifiedAt);
    }

    [Fact]
    public async Task Security_events_cannot_be_updated_or_deleted()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var securityEvent = new SecurityEvent(Guid.NewGuid(), "LoginSucceeded", SecurityEventOutcome.Succeeded,
            "user-1", null, null, Now, "correlation-1", null, null, "{}");

        db.SecurityEvents.Add(securityEvent);
        await db.SaveChangesAsync();
        db.SecurityEvents.Remove(securityEvent);

        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}
