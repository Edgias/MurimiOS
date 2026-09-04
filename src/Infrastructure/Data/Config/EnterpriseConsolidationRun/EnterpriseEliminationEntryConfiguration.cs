using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EnterpriseConsolidationRun;
using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using EnterpriseEliminationEntry = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseEliminationEntry;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
internal static class EnterpriseEliminationEntryConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var elimination = modelBuilder.Entity<EnterpriseEliminationEntry>();

        elimination.ToTable("consolidation_elimination_entries", "finance");

        context.ConfigureTenantEntity(elimination);

        elimination.Property(x => x.IntercompanyReference).HasMaxLength(160);

        elimination.Property(x => x.SellerRevenue).HasPrecision(28, 2);

        elimination.Property(x => x.BuyerExpense).HasPrecision(28, 2);

        elimination.Property(x => x.EliminatedAmount).HasPrecision(28, 2);

        elimination.Property(x => x.Difference).HasPrecision(28, 2);

        elimination.Property(x => x.Tolerance).HasPrecision(18, 2);

        elimination.Property(x => x.SellerTransactionIdsJson).HasColumnType("jsonb");

        elimination.Property(x => x.BuyerTransactionIdsJson).HasColumnType("jsonb");

        elimination.HasIndex(x => new { x.TenantId, x.ConsolidationRunId, x.RuleId, x.IntercompanyReference }).IsUnique();

        elimination.HasOne<EnterpriseConsolidationRun>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ConsolidationRunId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        elimination.HasOne<IntercompanyEliminationRule>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.RuleId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
