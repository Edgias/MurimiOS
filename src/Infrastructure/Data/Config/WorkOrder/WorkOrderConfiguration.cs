using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class WorkOrderConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var workOrder = modelBuilder.Entity<WorkOrder>();

        workOrder.ToTable("work_orders", "operations");

        context.ConfigureTenantEntity(workOrder);

        workOrder.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_operations_work_orders_tenant_id_id");

        workOrder.Property(x => x.Number).HasMaxLength(80);

        workOrder.Property(x => x.Title).HasMaxLength(200);

        workOrder.Property(x => x.WorkType).HasMaxLength(80);

        workOrder.Property(x => x.Instructions).HasMaxLength(4_000);

        workOrder.Property(x => x.CreatedBy).HasMaxLength(240);

        workOrder.Property(x => x.VerifiedBy).HasMaxLength(240);

        workOrder.Property(x => x.RejectedBy).HasMaxLength(240);

        workOrder.Property(x => x.RejectionReason).HasMaxLength(2_000);

        workOrder.Property(x => x.CancelledBy).HasMaxLength(240);

        workOrder.Property(x => x.CancellationReason).HasMaxLength(2_000);

        workOrder.Property(x => x.CompletionNotes).HasMaxLength(4_000);

        workOrder.HasIndex(x => new { x.TenantId, x.FarmId, x.Number }).IsUnique();

        workOrder.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.ScheduledStart });

        workOrder.HasIndex(x => new { x.TenantId, x.ScheduledStart, x.ScheduledEnd });

        workOrder.HasIndex(x => new { x.TenantId, x.CropPlanId, x.PlannedOperationId }).IsUnique()
            .HasFilter("planned_operation_id IS NOT NULL AND status <> 9");

        workOrder.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<FarmField>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<FieldBoundaryVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<CropZone>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.LivestockSubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<CropPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropPlanId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<PlannedCropOperation>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropPlanId, x.PlannedOperationId })
            .HasPrincipalKey(x => new { x.TenantId, x.PlanId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<WorkOrder>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PreviousWorkOrderId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        workOrder.HasOne<WorkOrder>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SupersededByWorkOrderId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
