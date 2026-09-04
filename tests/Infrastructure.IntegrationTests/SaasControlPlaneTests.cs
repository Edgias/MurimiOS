using EdgIQ.MurimiOS.Domain.TenantAccount;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using EdgIQ.MurimiOS.Domain.BillingAccount;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;
using BillingInvoiceStatus = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoiceStatus;
using EntitlementEnforcement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementEnforcement;
using EntitlementKind = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementKind;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SaasPlanStatus = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanStatus;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;
using TenantAccountStatus = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccountStatus;
using BillingInterval = global::EdgIQ.MurimiOS.Domain.TenantSubscription.BillingInterval;
using SubscriptionStatus = global::EdgIQ.MurimiOS.Domain.TenantSubscription.SubscriptionStatus;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
using UsageMeterEvent = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterEvent;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class SaasControlPlaneTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-09-01T08:00:00Z");

    [Fact]
    public void Tenant_account_has_explicit_provision_activate_suspend_resume_and_close_lifecycle()
    {
        var account = new TenantAccount(Guid.NewGuid(), "Murimi Farms (Pvt) Ltd", "Murimi Farms", "murimi-farms",
            "zw", "usd", "Africa/Harare", Now, "owner");
        Assert.Equal(TenantAccountStatus.Provisioning, account.Status);
        account.Activate(Now.AddMinutes(1));
        account.Suspend(Now.AddHours(1), "Invoice overdue");
        account.Activate(Now.AddHours(2));
        account.Close(Now.AddDays(1), "Customer requested closure");
        Assert.Equal(TenantAccountStatus.Closed, account.Status);
        Assert.Throws<InvalidOperationException>(() => account.Activate(Now.AddDays(2)));
    }

    [Fact]
    public void Versioned_plan_and_entitlement_validate_commercial_limits()
    {
        Guid planId = Guid.NewGuid();
        var plan = new SaasPlanVersion(planId, Guid.NewGuid(), "producer-pro", "Producer Pro", 1,
            BillingInterval.Annual, 500, "usd", Now, null, Now, "owner");
        var feature = new PlanEntitlement(Guid.NewGuid(), planId, "crop-planning", EntitlementKind.Feature,
            null, "enabled", EntitlementEnforcement.Hard);
        var limit = new PlanEntitlement(Guid.NewGuid(), planId, "managed-hectares", EntitlementKind.Limit,
            1_000, "hectares", EntitlementEnforcement.Hard);
        plan.Publish(Now.AddMinutes(1));
        Assert.Equal(SaasPlanStatus.Published, plan.Status);
        Assert.Null(feature.IncludedAmount);
        Assert.Equal(1_000, limit.IncludedAmount);
        Assert.Throws<ArgumentException>(() => new PlanEntitlement(Guid.NewGuid(), planId, "bad",
            EntitlementKind.Feature, 1, "enabled", EntitlementEnforcement.Hard));
    }

    [Fact]
    public void Trial_subscription_can_activate_renew_and_cancel()
    {
        var subscription = new TenantSubscription(Guid.NewGuid(), Guid.NewGuid(), Now, Now.AddDays(30),
            Now.AddDays(14), "sub_test", Now, "owner");
        Assert.Equal(SubscriptionStatus.Trialing, subscription.Status);
        subscription.Activate(Now.AddDays(14));
        subscription.Renew(Now.AddDays(30), Now.AddDays(60));
        subscription.ScheduleCancellation(Now.AddDays(60));
        subscription.Renew(Now.AddDays(60), Now.AddDays(90));
        Assert.Equal(SubscriptionStatus.Cancelled, subscription.Status);
        Assert.Equal(Now.AddDays(60), subscription.EndedAt);
    }

    [Fact]
    public void Usage_period_accumulates_and_usage_events_are_append_only()
    {
        var period = new UsageMeterPeriod(Guid.NewGuid(), "api-requests", "requests", Now, Now.AddMonths(1), 10, Now);
        period.Add(5, Now.AddMinutes(1));
        Assert.Equal(15, period.Quantity);
        Assert.Throws<ArgumentOutOfRangeException>(() => period.Add(0, Now));
    }

    [Fact]
    public void Invoice_totals_and_terminal_transitions_are_enforced()
    {
        var invoice = new BillingInvoice(Guid.NewGuid(), Guid.NewGuid(), null, "INV-001", null,
            100, 15, 115, "usd", Now, Now.AddDays(14));
        invoice.MarkPaid(Now.AddDays(1));
        Assert.Equal(BillingInvoiceStatus.Paid, invoice.Status);
        Assert.Throws<InvalidOperationException>(() => invoice.Void(Now.AddDays(2), "Cannot void paid invoice"));
        Assert.Throws<ArgumentException>(() => new BillingInvoice(Guid.NewGuid(), Guid.NewGuid(), null, "INV-002",
            null, 100, 15, 114, "USD", Now, Now.AddDays(14)));
    }

    [Fact]
    public async Task Usage_events_cannot_be_changed_or_deleted()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var usage = new UsageMeterEvent(Guid.NewGuid(), "command-1", "api-requests", 1, "requests", Now, "api", "{}");
        db.UsageMeterEvents.Add(usage); await db.SaveChangesAsync();
        db.UsageMeterEvents.Remove(usage);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    { public Guid TenantId { get; } = tenantId; public bool IsResolved => true; }
}
