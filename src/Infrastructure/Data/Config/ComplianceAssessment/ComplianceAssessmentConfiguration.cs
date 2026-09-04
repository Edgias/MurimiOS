using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAssessment;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class ComplianceAssessmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var assessment = modelBuilder.Entity<ComplianceAssessment>();

        assessment.ToTable("assessments", "compliance");

        context.ConfigureTenantEntity(assessment);

        assessment.Property(x => x.AssessmentCode).HasMaxLength(80);

        assessment.Property(x => x.Auditor).HasMaxLength(200);

        assessment.Property(x => x.ScopeJson).HasColumnType("jsonb");

        assessment.Property(x => x.ScorePercent).HasPrecision(8, 4);

        assessment.HasIndex(x => new { x.TenantId, x.AssessmentCode }).IsUnique();

        assessment.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.ScheduledOn });

        assessment.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        assessment.HasOne<ComplianceStandardVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.StandardVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
