using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsProjectionRun;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;

using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class AnalyticsFactConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var fact = modelBuilder.Entity<AnalyticsFact>();

        fact.ToTable("facts", "analytics");

        context.ConfigureTenantEntity(fact);

        fact.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_analytics_facts_tenant_id_id");

        fact.Property(x => x.MetricCode).HasMaxLength(120);

        fact.Property(x => x.Value).HasPrecision(28, 8);

        fact.Property(x => x.Unit).HasMaxLength(40);

        fact.Property(x => x.DataQualityPercent).HasPrecision(8, 4);

        fact.Property(x => x.FormulaVersion).HasMaxLength(60);

        fact.Property(x => x.GrainKey).HasMaxLength(300);

        fact.Property(x => x.Crop).HasMaxLength(120);

        fact.Property(x => x.Variety).HasMaxLength(120);

        fact.Property(x => x.Species).HasMaxLength(80);

        fact.Property(x => x.DimensionsJson).HasColumnType("jsonb");

        fact.Property(x => x.ConfidencePercent).HasPrecision(8, 4);

        fact.Property(x => x.LowerBound).HasPrecision(28, 8);

        fact.Property(x => x.UpperBound).HasPrecision(28, 8);

        fact.HasIndex(x => new { x.TenantId, x.ProjectionRunId, x.MetricCode, x.ValueKind, x.GrainKey }).IsUnique();

        fact.HasIndex(x => new { x.TenantId, x.FarmId, x.MetricCode, x.PeriodStart, x.PeriodEnd });

        fact.HasIndex(x => new { x.TenantId, x.OrganisationNodeId, x.MetricCode, x.PeriodEnd });

        fact.HasOne<AnalyticsProjectionRun>().WithMany().HasForeignKey(x => new { x.TenantId, x.ProjectionRunId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        fact.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        fact.HasOne<OrganisationNode>().WithMany().HasForeignKey(x => new { x.TenantId, x.OrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
