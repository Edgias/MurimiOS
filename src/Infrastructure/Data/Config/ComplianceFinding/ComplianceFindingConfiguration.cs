using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceFinding;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
internal static class ComplianceFindingConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var finding = modelBuilder.Entity<ComplianceFinding>();

        finding.ToTable("findings", "compliance");

        context.ConfigureTenantEntity(finding);

        finding.Property(x => x.Title).HasMaxLength(240);

        finding.Property(x => x.Description).HasMaxLength(4_000);

        finding.Property(x => x.ClosedBy).HasMaxLength(240);

        finding.Property(x => x.WaiverReason).HasMaxLength(2_000);

        finding.HasIndex(x => new { x.TenantId, x.AssessmentId, x.ControlPointId }).IsUnique();

        finding.HasIndex(x => new { x.TenantId, x.Status, x.Severity, x.DueOn });

        finding.HasOne<ComplianceAssessment>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AssessmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        finding.HasOne<ComplianceControlPoint>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ControlPointId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
