using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IntercompanyEliminationRule;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class IntercompanyEliminationRuleConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var rule = modelBuilder.Entity<IntercompanyEliminationRule>();

        rule.ToTable("intercompany_elimination_rules", "finance");

        context.ConfigureTenantEntity(rule);

        rule.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_finance_elimination_rules_tenant_id_id");

        rule.Property(x => x.RuleKey).HasMaxLength(120);

        rule.Property(x => x.Name).HasMaxLength(240);

        rule.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        rule.Property(x => x.BalancingTolerance).HasPrecision(18, 2);

        rule.Property(x => x.CreatedBy).HasMaxLength(240);

        rule.Property(x => x.ApprovedBy).HasMaxLength(240);

        rule.Property(x => x.ApprovalReference).HasMaxLength(240);

        rule.Property(x => x.RetiredBy).HasMaxLength(240);

        rule.HasIndex(x => new { x.TenantId, x.RuleKey, x.RuleVersion }).IsUnique();

        rule.HasIndex(x => new { x.TenantId, x.ConsolidationOrganisationNodeId, x.Status });

        rule.HasIndex(x => new { x.TenantId, x.SellerCompanyId, x.BuyerCompanyId, x.Currency }).IsUnique()
            .HasFilter("status = 2");

        rule.HasOne<OrganisationNode>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ConsolidationOrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        rule.HasOne<LegalCompany>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SellerCompanyId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        rule.HasOne<LegalCompany>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BuyerCompanyId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
