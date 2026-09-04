using EdgIQ.MurimiOS.Domain.CropPlan;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanFinancialEntry = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanFinancialEntry;
internal static class CropPlanFinancialEntryConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var financial = modelBuilder.Entity<CropPlanFinancialEntry>();

        financial.ToTable("financial_entries", "planning");

        context.ConfigureTenantEntity(financial);

        financial.Property(x => x.Category).HasMaxLength(120);

        financial.Property(x => x.Amount).HasPrecision(18, 2);

        financial.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        financial.Property(x => x.Reference).HasMaxLength(160);

        financial.Property(x => x.Notes).HasMaxLength(2_000);

        financial.Property(x => x.RecordedBy).HasMaxLength(240);

        financial.HasIndex(x => new { x.TenantId, x.PlanId, x.OccurredAt });

        financial.HasIndex(x => new { x.TenantId, x.Reference }).IsUnique();

        financial.HasOne<CropPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        financial.HasOne<CropPlanAllocation>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId, x.AllocationId })
            .HasPrincipalKey(x => new { x.TenantId, x.PlanId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
