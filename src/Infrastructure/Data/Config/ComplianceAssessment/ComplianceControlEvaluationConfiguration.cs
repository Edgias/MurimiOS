using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAssessment;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;
using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
internal static class ComplianceControlEvaluationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var evaluation = modelBuilder.Entity<ComplianceControlEvaluation>();

        evaluation.ToTable("control_evaluations", "compliance");

        context.ConfigureTenantEntity(evaluation);

        evaluation.Property(x => x.ScorePercent).HasPrecision(8, 4);

        evaluation.Property(x => x.EvaluatorVersion).HasMaxLength(60);

        evaluation.Property(x => x.EvaluatedBy).HasMaxLength(240);

        evaluation.Property(x => x.Notes).HasMaxLength(4_000);

        evaluation.Property(x => x.EvidenceSnapshotJson).HasColumnType("jsonb");

        evaluation.HasIndex(x => new { x.TenantId, x.AssessmentId, x.ControlPointId }).IsUnique();

        evaluation.HasOne<ComplianceAssessment>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AssessmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        evaluation.HasOne<ComplianceControlPoint>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ControlPointId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
