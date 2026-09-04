using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldRiskRule;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldRiskAssessment = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskAssessment;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
internal static class FieldRiskAssessmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var assessment = modelBuilder.Entity<FieldRiskAssessment>();

        assessment.ToTable("risk_assessments", "insights");

        context.ConfigureTenantEntity(assessment);

        assessment.Property(x => x.ObservedValue).HasPrecision(18, 6);

        assessment.Property(x => x.EvaluatorVersion).HasMaxLength(40);

        assessment.Property(x => x.InputSnapshotJson).HasColumnType("jsonb");

        assessment.HasIndex(x => new { x.TenantId, x.FieldId, x.AssessedAt });

        assessment.HasIndex(x => new { x.TenantId, x.RuleId, x.FieldId, x.AssessedAt });

        assessment.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        assessment.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        assessment.HasOne<FieldBoundaryVersion>().WithMany().HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        assessment.HasOne<FieldRiskRule>().WithMany().HasForeignKey(x => new { x.TenantId, x.RuleId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
