using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SatelliteFieldStatistic;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
internal static class SatelliteFieldStatisticConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var satellite = modelBuilder.Entity<SatelliteFieldStatistic>();

        satellite.ToTable("satellite_field_statistics", "insights");

        context.ConfigureTenantEntity(satellite);

        satellite.Property(x => x.ProviderAcquisitionId).HasMaxLength(240);

        satellite.Property(x => x.CustomIndex).HasMaxLength(40);

        foreach (string property in new[] { nameof(SatelliteFieldStatistic.Mean), nameof(SatelliteFieldStatistic.Minimum), nameof(SatelliteFieldStatistic.Maximum), nameof(SatelliteFieldStatistic.StandardDeviation), nameof(SatelliteFieldStatistic.ValidPixelPercent), nameof(SatelliteFieldStatistic.CloudPercent) })
            satellite.Property(property).HasPrecision(10, 5);

        satellite.HasIndex(x => new { x.TenantId, x.FeedId, x.ProviderAcquisitionId, x.Index, x.CustomIndex }).IsUnique();

        satellite.HasIndex(x => new { x.TenantId, x.FieldId, x.Index, x.AcquiredAt });

        satellite.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        satellite.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        satellite.HasOne<FieldBoundaryVersion>().WithMany().HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        satellite.HasOne<IntegrationFeed>().WithMany().HasForeignKey(x => new { x.TenantId, x.FeedId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        satellite.HasOne<IngestionRecord>().WithMany().HasForeignKey(x => new { x.TenantId, x.IngestionRecordId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
