using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAuditPackExport;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceAuditPackExport = global::EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport.ComplianceAuditPackExport;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class ComplianceAuditPackExportConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var auditPack = modelBuilder.Entity<ComplianceAuditPackExport>();

        auditPack.ToTable("audit_pack_exports", "compliance");

        context.ConfigureTenantEntity(auditPack);

        auditPack.Property(x => x.ScopeStatement).HasMaxLength(4_000);

        auditPack.Property(x => x.ObjectKey).HasMaxLength(1_000);

        auditPack.Property(x => x.ArtifactSha256).HasMaxLength(64).IsFixedLength();

        auditPack.Property(x => x.ManifestSha256).HasMaxLength(64).IsFixedLength();

        auditPack.Property(x => x.SignatureAlgorithm).HasMaxLength(80);

        auditPack.Property(x => x.SignatureKeyId).HasMaxLength(160);

        auditPack.Property(x => x.SignatureBase64).HasMaxLength(2_000);

        auditPack.Property(x => x.ManifestJson).HasColumnType("text");

        auditPack.Property(x => x.GeneratedBy).HasMaxLength(240);

        auditPack.HasIndex(x => new { x.TenantId, x.AssessmentId, x.Revision }).IsUnique();

        auditPack.HasIndex(x => new { x.TenantId, x.FarmId, x.GeneratedAt });

        auditPack.HasOne<ComplianceAssessment>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AssessmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        auditPack.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        auditPack.HasOne<ComplianceStandardVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.StandardVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
