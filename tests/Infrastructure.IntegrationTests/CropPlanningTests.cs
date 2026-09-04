using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanFinancialEntry = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanFinancialEntry;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using FinancialEntryType = global::EdgIQ.MurimiOS.Domain.CropPlan.FinancialEntryType;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class CropPlanningTests
{
    [Fact]
    public void Plan_requires_submission_before_approval_and_preserves_revision_history()
    {
        Guid planId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var plan = new CropPlan(planId, Guid.NewGuid(), planId, Guid.NewGuid(), Guid.NewGuid(),
            "2026 maize plan", "Baseline", "ZWL", 1, null, now, "planner");

        Assert.Throws<InvalidOperationException>(() => plan.Approve(now, "manager"));
        plan.Submit(now.AddMinutes(1), "planner");
        plan.Approve(now.AddMinutes(2), "manager");
        Assert.Equal(CropPlanStatus.Approved, plan.Status);

        Guid replacement = Guid.NewGuid();
        plan.Supersede(replacement, now.AddDays(1));
        Assert.Equal(CropPlanStatus.Superseded, plan.Status);
        Assert.Equal(replacement, plan.SupersededByPlanId);
    }

    [Fact]
    public void Allocation_and_resource_budget_canonicalize_units_and_calculate_financials()
    {
        Guid planId = Guid.NewGuid();
        Guid allocationId = Guid.NewGuid();
        var allocation = new CropPlanAllocation(allocationId, planId, Guid.NewGuid(), Guid.NewGuid(), null,
            "Maize", "SC 727", 10m, "acre", 8m, "t", 0.45m, "USD",
            new DateOnly(2026, 9, 1), new DateOnly(2027, 4, 30));
        var resource = new CropPlanResourceBudget(Guid.NewGuid(), planId, allocationId, Guid.NewGuid(),
            PlannedResourceType.Input, Guid.NewGuid(), "Compound D", 1.5m, "t", 0.80m, "USD");

        Assert.Equal(4.0468564224m, allocation.PlannedAreaHa);
        Assert.Equal(8000m, allocation.TargetYield);
        Assert.Equal(3600m, allocation.PlannedRevenue);
        Assert.Equal(1500m, resource.Quantity);
        Assert.Equal(1200m, resource.PlannedCost);
    }

    [Fact]
    public void Product_rule_enforces_crop_rate_authorization_and_pre_harvest_interval()
    {
        Guid planId = Guid.NewGuid(); Guid allocationId = Guid.NewGuid(); Guid inputId = Guid.NewGuid();
        var allocation = new CropPlanAllocation(allocationId, planId, Guid.NewGuid(), Guid.NewGuid(), null,
            "Maize", null, 10m, "ha", 10m, "t", 1m, "USD",
            new DateOnly(2026, 9, 1), new DateOnly(2027, 3, 1));
        var validBudget = new CropPlanResourceBudget(Guid.NewGuid(), planId, allocationId, Guid.NewGuid(),
            PlannedResourceType.Input, inputId, "Herbicide", 20m, "L", 5m, "USD");
        var excessiveBudget = new CropPlanResourceBudget(Guid.NewGuid(), planId, allocationId, Guid.NewGuid(),
            PlannedResourceType.Input, inputId, "Herbicide", 30m, "L", 5m, "USD");
        var rule = new CropProductApplicationRule(Guid.NewGuid(), inputId, "Maize", "ZW",
            2.5m, "L", 24, 14, new DateOnly(2026, 1, 1), new DateOnly(2027, 12, 31), "REG-1");

        rule.EnsurePermitted(allocation, validBudget, new DateOnly(2026, 10, 1));
        Assert.Throws<InvalidOperationException>(() => rule.EnsurePermitted(allocation, excessiveBudget, new DateOnly(2026, 10, 1)));
        Assert.Throws<InvalidOperationException>(() => rule.EnsurePermitted(allocation, validBudget, new DateOnly(2027, 2, 20)));

        var application = new PlannedCropOperation(Guid.NewGuid(), planId, allocationId, "InputApplication",
            new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 1), 1, null);
        var sameDayFieldWork = new PlannedCropOperation(Guid.NewGuid(), planId, allocationId, "Scouting",
            new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 1), 2, null);
        var permittedFieldWork = new PlannedCropOperation(Guid.NewGuid(), planId, allocationId, "Scouting",
            new DateOnly(2026, 10, 2), new DateOnly(2026, 10, 2), 3, null);
        Assert.Throws<InvalidOperationException>(() =>
            rule.EnsureReEntryInterval(application, [application, sameDayFieldWork]));
        rule.EnsureReEntryInterval(application, [application, permittedFieldWork]);
    }

    [Fact]
    public async Task Actual_financial_entries_are_append_only()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var entry = new CropPlanFinancialEntry(Guid.NewGuid(), Guid.NewGuid(), null, FinancialEntryType.Cost,
            "Contractor", 250m, "USD", DateTimeOffset.UtcNow, "INV-1", null, "planner");
        db.CropPlanFinancialEntries.Add(entry);
        await db.SaveChangesAsync();
        db.CropPlanFinancialEntries.Remove(entry);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Planning_permissions_separate_drafting_from_approval()
    {
        Assert.Contains(PlatformPermissions.PlanningApprove, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.PlanningWrite, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.DoesNotContain(PlatformPermissions.PlanningApprove, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.Contains(PlatformPermissions.PlanningRead, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.DoesNotContain(PlatformPermissions.PlanningRead, PlatformPermissions.ForRole(TenantRole.Buyer));
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}
