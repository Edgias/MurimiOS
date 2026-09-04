using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
internal static class CropPlanResourceBudgetConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var budget = modelBuilder.Entity<CropPlanResourceBudget>();

        budget.ToTable("resource_budgets", "planning");

        context.ConfigureTenantEntity(budget);

        budget.Property(x => x.ResourceName).HasMaxLength(160);

        budget.Property(x => x.Quantity).HasPrecision(18, 4);

        budget.Property(x => x.Unit).HasMaxLength(30);

        budget.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        budget.Property(x => x.EnteredUnit).HasMaxLength(30);

        budget.Property(x => x.UnitCostPerCanonicalUnit).HasPrecision(18, 6);

        budget.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        budget.Ignore(x => x.PlannedCost);

        budget.HasIndex(x => new { x.TenantId, x.PlanId, x.AllocationId, x.ResourceType });

        budget.HasOne<CropPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        budget.HasOne<CropPlanAllocation>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId, x.AllocationId })
            .HasPrincipalKey(x => new { x.TenantId, x.PlanId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        budget.HasOne<PlannedCropOperation>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId, x.PlannedOperationId })
            .HasPrincipalKey(x => new { x.TenantId, x.PlanId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        budget.HasOne<AgriculturalInput>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AgriculturalInputId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
