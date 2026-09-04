using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
internal static class CropPlanAllocationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var allocation = modelBuilder.Entity<CropPlanAllocation>();

        allocation.ToTable("allocations", "planning");

        context.ConfigureTenantEntity(allocation);

        allocation.HasAlternateKey(x => new { x.TenantId, x.PlanId, x.Id })
            .HasName("ak_planning_allocations_tenant_plan_id");

        allocation.Property(x => x.Crop).HasMaxLength(120);

        allocation.Property(x => x.Variety).HasMaxLength(120);

        allocation.Property(x => x.PlannedAreaHa).HasPrecision(14, 4);

        allocation.Property(x => x.EnteredArea).HasPrecision(14, 4);

        allocation.Property(x => x.EnteredAreaUnit).HasMaxLength(30);

        allocation.Property(x => x.TargetYield).HasPrecision(18, 4);

        allocation.Property(x => x.TargetYieldUnit).HasMaxLength(30);

        allocation.Property(x => x.EnteredTargetYield).HasPrecision(18, 4);

        allocation.Property(x => x.EnteredTargetYieldUnit).HasMaxLength(30);

        allocation.Property(x => x.TargetPricePerCanonicalUnit).HasPrecision(18, 6);

        allocation.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        allocation.Ignore(x => x.PlannedRevenue);

        allocation.HasIndex(x => new { x.TenantId, x.PlanId, x.FieldId });

        allocation.HasIndex(x => new { x.TenantId, x.PlanId, x.CropZoneId }).IsUnique();

        allocation.HasOne<CropPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        allocation.HasOne<FarmField>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        allocation.HasOne<FieldBoundaryVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        allocation.HasOne<CropZone>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
