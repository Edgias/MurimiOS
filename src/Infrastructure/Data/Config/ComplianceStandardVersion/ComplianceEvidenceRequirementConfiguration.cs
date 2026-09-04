using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceStandardVersion;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceEvidenceRequirement = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceEvidenceRequirement;
internal static class ComplianceEvidenceRequirementConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var requirement = modelBuilder.Entity<ComplianceEvidenceRequirement>();

        requirement.ToTable("evidence_requirements", "compliance");

        context.ConfigureTenantEntity(requirement);

        requirement.Property(x => x.Code).HasMaxLength(80);

        requirement.Property(x => x.Name).HasMaxLength(200);

        requirement.Property(x => x.Instructions).HasMaxLength(2_000);

        requirement.HasIndex(x => new { x.TenantId, x.ControlPointId, x.Code }).IsUnique();

        requirement.HasOne<ComplianceControlPoint>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ControlPointId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
