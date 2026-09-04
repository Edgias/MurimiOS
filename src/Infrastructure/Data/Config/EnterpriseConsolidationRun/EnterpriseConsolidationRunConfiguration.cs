using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EnterpriseConsolidationRun;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class EnterpriseConsolidationRunConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var consolidation = modelBuilder.Entity<EnterpriseConsolidationRun>();

        consolidation.ToTable("consolidation_runs", "finance");

        context.ConfigureTenantEntity(consolidation);

        consolidation.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_finance_consolidation_runs_tenant_id_id");

        consolidation.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        consolidation.Property(x => x.GrossExpenses).HasPrecision(28, 2);

        consolidation.Property(x => x.GrossRevenue).HasPrecision(28, 2);

        consolidation.Property(x => x.EliminatedExpenses).HasPrecision(28, 2);

        consolidation.Property(x => x.EliminatedRevenue).HasPrecision(28, 2);

        consolidation.Property(x => x.ConsolidatedExpenses).HasPrecision(28, 2);

        consolidation.Property(x => x.ConsolidatedRevenue).HasPrecision(28, 2);

        consolidation.Property(x => x.ConsolidatedMargin).HasPrecision(28, 2);

        consolidation.Property(x => x.SourceSha256).HasMaxLength(64).IsFixedLength();

        consolidation.Property(x => x.RequestedBy).HasMaxLength(240);

        consolidation.HasIndex(x => new { x.TenantId, x.OrganisationNodeId, x.PeriodEnd });

        consolidation.HasOne<OrganisationNode>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.OrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
