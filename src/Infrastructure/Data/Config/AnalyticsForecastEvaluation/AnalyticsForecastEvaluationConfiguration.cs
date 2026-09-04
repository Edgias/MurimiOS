using EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsForecastEvaluation;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;

using AnalyticsForecastEvaluation = global::EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation.AnalyticsForecastEvaluation;
using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class AnalyticsForecastEvaluationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var evaluation = modelBuilder.Entity<AnalyticsForecastEvaluation>();

        evaluation.ToTable("forecast_evaluations", "analytics"); 
context.ConfigureTenantEntity(evaluation);

        evaluation.Property(x => x.MetricCode).HasMaxLength(120); 
evaluation.Property(x => x.GrainKey).HasMaxLength(300);

        evaluation.Property(x => x.ForecastValue).HasPrecision(28, 8); 
evaluation.Property(x => x.ActualValue).HasPrecision(28, 8);

        evaluation.Property(x => x.AbsoluteError).HasPrecision(28, 8); 
evaluation.Property(x => x.AbsolutePercentageError).HasPrecision(18, 8);

        evaluation.Property(x => x.Bias).HasPrecision(28, 8); 
evaluation.Property(x => x.Unit).HasMaxLength(40);

        evaluation.Property(x => x.ModelVersion).HasMaxLength(120);

        evaluation.HasIndex(x => new { x.TenantId, x.ForecastFactId, x.ActualFactId }).IsUnique();

        evaluation.HasIndex(x => new { x.TenantId, x.FarmId, x.MetricCode, x.EvaluatedAt });

        evaluation.HasOne<AnalyticsFact>().WithMany().HasForeignKey(x => new { x.TenantId, x.ForecastFactId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        evaluation.HasOne<AnalyticsFact>().WithMany().HasForeignKey(x => new { x.TenantId, x.ActualFactId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        evaluation.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
