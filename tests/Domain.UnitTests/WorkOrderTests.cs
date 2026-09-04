using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.FieldOperationRecord;

namespace EdgIQ.MurimiOS.Domain.UnitTests;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderPriority = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderPriority;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
using WorkOrderStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatus;
using WorkResourceStatus = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceStatus;
using WorkResourceType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResourceType;

public sealed class WorkOrderTests
{
    [Fact]
    public void Work_order_follows_planned_assigned_execution_and_verification_lifecycle()
    {
        DateTimeOffset now = DateTimeOffset.Parse("2026-08-30T06:00:00Z");
        WorkOrder order = CreateOrder(now);

        order.Plan(now);
        order.MarkAssigned(now.AddMinutes(5));
        order.Start(now.AddHours(1));
        order.Complete(now.AddHours(3), "Completed to specification.");
        order.Verify(now.AddHours(4), "farm-manager");

        Assert.Equal(WorkOrderStatus.Verified, order.Status);
        Assert.Equal("Completed to specification.", order.CompletionNotes);
        Assert.Equal("farm-manager", order.VerifiedBy);
        Assert.Throws<InvalidOperationException>(() => order.Cancel(now.AddHours(5), "manager", "Too late"));
    }

    [Fact]
    public void Rejected_work_can_be_reopened_and_completed_again()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        WorkOrder order = CreateOrder(now);
        order.Plan(now); order.MarkAssigned(now); order.Start(now); order.Complete(now, null);
        order.Reject(now, "supervisor", "Evidence is incomplete.");

        order.Reopen(now.AddMinutes(1));
        order.Start(now.AddMinutes(2));
        order.Complete(now.AddMinutes(3), "Evidence attached.");

        Assert.Equal(WorkOrderStatus.Completed, order.Status);
        Assert.Null(order.RejectionReason);
        Assert.NotNull(order.RejectedAt);
    }

    [Fact]
    public void Verified_work_is_corrected_by_supersession_not_mutation()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        WorkOrder order = CreateOrder(now);
        order.Plan(now); order.MarkAssigned(now); order.Start(now); order.Complete(now, null); order.Verify(now, "manager");
        Guid replacementId = Guid.NewGuid();

        order.Supersede(replacementId);

        Assert.Equal(WorkOrderStatus.Superseded, order.Status);
        Assert.Equal(replacementId, order.SupersededByWorkOrderId);
    }

    [Fact]
    public void Field_target_requires_a_boundary_snapshot_and_cannot_mix_with_livestock()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        Assert.Throws<ArgumentException>(() => new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "WO-1", "Scout",
            "Scouting", WorkOrderPriority.Normal, now, now.AddHours(1), "Inspect", "manager", now,
            fieldId: Guid.NewGuid()));
        Assert.Throws<ArgumentException>(() => new WorkOrder(Guid.NewGuid(), Guid.NewGuid(), "WO-2", "Scout",
            "Scouting", WorkOrderPriority.Normal, now, now.AddHours(1), "Inspect", "manager", now,
            fieldId: Guid.NewGuid(), boundaryVersionId: Guid.NewGuid(), livestockSubjectId: Guid.NewGuid()));
    }

    [Fact]
    public void Resource_reservation_uses_canonical_units_and_has_one_way_consumption()
    {
        var resource = new WorkOrderResource(Guid.NewGuid(), Guid.NewGuid(), WorkResourceType.Input, "Fertiliser",
            1m, "t", 0.5m, "usd", Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(1_000m, resource.Quantity);
        Assert.Equal("kg", resource.Unit);
        Assert.Equal("USD", resource.Currency);
        Assert.Equal(500m, resource.PlannedCost);
        resource.Consume();
        Assert.Equal(WorkResourceStatus.Consumed, resource.Status);
        Assert.Throws<InvalidOperationException>(resource.Consume);
        Assert.Throws<InvalidOperationException>(resource.Release);
    }

    [Fact]
    public void Operations_permissions_separate_execution_assignment_and_verification()
    {
        Assert.Contains(PlatformPermissions.OperationsVerify, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.OperationsAssign, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.Contains(PlatformPermissions.OperationsWrite, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.DoesNotContain(PlatformPermissions.OperationsAssign, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.DoesNotContain(PlatformPermissions.OperationsVerify, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.Contains(PlatformPermissions.OperationsRead, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private static WorkOrder CreateOrder(DateTimeOffset now) => new(Guid.NewGuid(), Guid.NewGuid(), "WO-TEST-1",
        "Plant maize", "Planting", WorkOrderPriority.High, now, now.AddHours(2), "Use approved seed rate.",
        "planner", now, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
}
