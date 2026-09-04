using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FinanceTransaction;
using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using CostAllocation = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.CostAllocation;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
internal static class CostAllocationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var allocation = modelBuilder.Entity<CostAllocation>();

        allocation.ToTable("cost_allocations", "finance");

        context.ConfigureTenantEntity(allocation);

        allocation.Property(x => x.Amount).HasPrecision(18, 2);

        allocation.Property(x => x.Percentage).HasPrecision(9, 6);

        allocation.Property(x => x.Notes).HasMaxLength(1_000);

        allocation.HasIndex(x => new { x.TenantId, x.TransactionId, x.CostCentreId }).IsUnique();

        allocation.HasIndex(x => new { x.TenantId, x.CostCentreId, x.TransactionId });

        allocation.HasOne<FinanceTransaction>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.TransactionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        allocation.HasOne<CostCentre>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CostCentreId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
