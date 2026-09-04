using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceStandardVersion;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
internal static class ComplianceStandardVersionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var standard = modelBuilder.Entity<ComplianceStandardVersion>();

        standard.ToTable("standard_versions", "compliance");

        context.ConfigureTenantEntity(standard);

        standard.Property(x => x.Code).HasMaxLength(60);

        standard.Property(x => x.Name).HasMaxLength(200);

        standard.Property(x => x.Scheme).HasMaxLength(80);

        standard.Property(x => x.VersionLabel).HasMaxLength(60);

        standard.Property(x => x.Jurisdiction).HasMaxLength(80);

        standard.Property(x => x.PackKey).HasMaxLength(120);

        standard.Property(x => x.SourceUri).HasMaxLength(1_000);

        standard.Property(x => x.ManifestSha256).HasMaxLength(64).IsFixedLength();

        standard.Property(x => x.ConformancePolicyJson).HasColumnType("jsonb");

        standard.Property(x => x.PublishedBy).HasMaxLength(240);

        standard.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_standard_versions_tenant_id_id");

        standard.HasIndex(x => new { x.TenantId, x.SeriesId, x.Revision }).IsUnique();

        standard.HasIndex(x => new { x.TenantId, x.Code, x.Status });

        standard.HasIndex(x => new { x.TenantId, x.PackKey, x.ManifestSha256 }).IsUnique()
            .HasFilter("pack_key IS NOT NULL AND manifest_sha256 IS NOT NULL");

    }
}
