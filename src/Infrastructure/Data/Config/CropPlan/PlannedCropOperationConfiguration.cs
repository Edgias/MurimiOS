using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
internal static class PlannedCropOperationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var operation = modelBuilder.Entity<PlannedCropOperation>();

        operation.ToTable("operations", "planning");

        context.ConfigureTenantEntity(operation);

        operation.HasAlternateKey(x => new { x.TenantId, x.PlanId, x.Id })
            .HasName("ak_planning_operations_tenant_plan_id");

        operation.Property(x => x.OperationType).HasMaxLength(80);

        operation.Property(x => x.Notes).HasMaxLength(2_000);

        operation.HasIndex(x => new { x.TenantId, x.PlanId, x.AllocationId, x.Sequence }).IsUnique();

        operation.HasIndex(x => new { x.TenantId, x.PlannedStart, x.PlannedEnd });

        operation.HasOne<CropPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        operation.HasOne<CropPlanAllocation>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId, x.AllocationId })
            .HasPrincipalKey(x => new { x.TenantId, x.PlanId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
