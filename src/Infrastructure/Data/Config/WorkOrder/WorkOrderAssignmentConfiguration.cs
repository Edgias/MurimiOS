using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderAssignment = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderAssignment;
internal static class WorkOrderAssignmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var assignment = modelBuilder.Entity<WorkOrderAssignment>();

        assignment.ToTable("work_order_assignments", "operations");

        context.ConfigureTenantEntity(assignment);

        assignment.Property(x => x.AssignmentRole).HasMaxLength(100);

        assignment.Property(x => x.AssignedBy).HasMaxLength(240);

        assignment.Property(x => x.UnassignedBy).HasMaxLength(240);

        assignment.HasIndex(x => new { x.TenantId, x.WorkOrderId, x.MembershipId }).IsUnique()
            .HasFilter("unassigned_at IS NULL");

        assignment.HasIndex(x => new { x.TenantId, x.WorkOrderId, x.IsPrimary }).IsUnique()
            .HasFilter("unassigned_at IS NULL AND is_primary");

        assignment.HasIndex(x => new { x.TenantId, x.MembershipId, x.UnassignedAt });

        assignment.HasOne<WorkOrder>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.WorkOrderId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        assignment.HasOne<TenantMembership>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.MembershipId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
