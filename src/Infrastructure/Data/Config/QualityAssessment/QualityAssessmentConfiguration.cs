using EdgIQ.MurimiOS.Domain.QualityAssessment;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.QualityAssessment;
using QualityAssessment = global::EdgIQ.MurimiOS.Domain.QualityAssessment.QualityAssessment;
internal static class QualityAssessmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var quality = modelBuilder.Entity<QualityAssessment>();

        quality.ToTable("quality_assessments", "traceability");

        context.ConfigureTenantEntity(quality);

        quality.Property(x => x.Metric).HasMaxLength(120);

        quality.Property(x => x.NumericResult).HasPrecision(18, 4);

        quality.Property(x => x.TextResult).HasMaxLength(500);

        quality.Property(x => x.Unit).HasMaxLength(30);

        quality.Property(x => x.Notes).HasMaxLength(2_000);

        quality.HasIndex(x => new { x.TenantId, x.ObjectType, x.ObjectId, x.AssessedAt });

    }
}
