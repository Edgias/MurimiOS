using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldRiskRule;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;

using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
internal static class FieldRiskRuleConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var rule = modelBuilder.Entity<FieldRiskRule>();

        rule.ToTable("risk_rules", "insights");

        context.ConfigureTenantEntity(rule);

        rule.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_field_risk_rules_tenant_id_id");

        rule.Property(x => x.RuleKey).HasMaxLength(120);

        rule.Property(x => x.Name).HasMaxLength(200);

        rule.Property(x => x.Crop).HasMaxLength(120);

        rule.Property(x => x.IssueCode).HasMaxLength(80);

        rule.Property(x => x.Threshold).HasPrecision(18, 6);

        rule.Property(x => x.Message).HasMaxLength(1_000);

        rule.Property(x => x.CreatedBy).HasMaxLength(240);

        rule.HasIndex(x => new { x.TenantId, x.RuleKey, x.RuleVersion }).IsUnique();

        rule.HasIndex(x => new { x.TenantId, x.RuleKey, x.Status }).IsUnique().HasFilter("status = 2");

        rule.HasIndex(x => new { x.TenantId, x.Status, x.EffectiveFrom, x.EffectiveTo });

    }
}
