using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.GeospatialImportJob;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using GeospatialImportFeature = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportFeature;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
internal static class GeospatialImportFeatureConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var feature = modelBuilder.Entity<GeospatialImportFeature>();

        feature.ToTable("geospatial_features", "imports");

        context.ConfigureTenantEntity(feature);

        feature.Property(x => x.SourceName).HasMaxLength(200);

        feature.Property(x => x.Boundary).HasColumnType("geography (multipolygon, 4326)");

        feature.Property(x => x.AreaHa).HasPrecision(14, 4)
            .HasComputedColumnSql("ST_Area(boundary) / 10000.0", stored: true);

        feature.Property(x => x.PropertiesJson).HasColumnType("jsonb");

        feature.Property(x => x.IssuesJson).HasColumnType("jsonb");

        feature.HasIndex(x => new { x.TenantId, x.JobId, x.Ordinal }).IsUnique();

        feature.HasIndex(x => new { x.TenantId, x.JobId, x.IsEligible });

        feature.HasOne<GeospatialImportJob>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.JobId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Cascade);

        feature.HasOne<FarmField>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AppliedFieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        feature.HasOne<FieldBoundaryVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AppliedBoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
