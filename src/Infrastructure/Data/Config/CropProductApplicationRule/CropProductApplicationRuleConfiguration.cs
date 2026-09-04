using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.CropProductApplicationRule;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropProductApplicationRule;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;

using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
internal static class CropProductApplicationRuleConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var rule = modelBuilder.Entity<CropProductApplicationRule>();

        rule.ToTable("product_application_rules", "planning");

        context.ConfigureTenantEntity(rule);

        rule.Property(x => x.Crop).HasMaxLength(120);

        rule.Property(x => x.Jurisdiction).HasMaxLength(80);

        rule.Property(x => x.MaximumRatePerHa).HasPrecision(18, 6);

        rule.Property(x => x.RateUnit).HasMaxLength(30);

        rule.Property(x => x.EnteredMaximumRatePerHa).HasPrecision(18, 6);

        rule.Property(x => x.EnteredRateUnit).HasMaxLength(30);

        rule.Property(x => x.AuthorizationReference).HasMaxLength(160);

        rule.HasIndex(x => new { x.TenantId, x.InputId, x.Crop, x.Jurisdiction, x.EffectiveFrom }).IsUnique();

        rule.HasIndex(x => new { x.TenantId, x.InputId, x.Crop, x.EffectiveFrom, x.EffectiveTo });

        rule.HasOne<AgriculturalInput>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.InputId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
