using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsDimensionSnapshot;
using AnalyticsDimensionSnapshot = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshot;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class AnalyticsDimensionSnapshotConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var dimension = modelBuilder.Entity<AnalyticsDimensionSnapshot>();

        dimension.ToTable("dimension_snapshots", "analytics"); 
context.ConfigureTenantEntity(dimension);

        dimension.Property(x => x.GrainKey).HasMaxLength(300); 
dimension.Property(x => x.NaturalKey).HasMaxLength(300);

        dimension.Property(x => x.AttributesJson).HasColumnType("jsonb"); 
dimension.Property(x => x.AttributesSha256).HasMaxLength(64).IsFixedLength();

        dimension.HasIndex(x => new { x.TenantId, x.ProjectionRunId, x.DimensionType, x.EntityId }).IsUnique();

        dimension.HasIndex(x => new { x.TenantId, x.DimensionType, x.EntityId, x.ValidOn });

        dimension.HasOne<AnalyticsProjectionRun>().WithMany().HasForeignKey(x => new { x.TenantId, x.ProjectionRunId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        dimension.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
