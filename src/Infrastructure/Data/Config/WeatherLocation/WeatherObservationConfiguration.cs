using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WeatherLocation;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;
internal static class WeatherObservationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var weather = modelBuilder.Entity<WeatherObservation>();

        weather.ToTable("weather_observations", "insights");

        context.ConfigureTenantEntity(weather);

        weather.Property(x => x.ProviderObservationId).HasMaxLength(240);

        foreach (string property in new[] { nameof(WeatherObservation.TemperatureC), nameof(WeatherObservation.RelativeHumidityPercent), nameof(WeatherObservation.PrecipitationMm), nameof(WeatherObservation.WindSpeedMps), nameof(WeatherObservation.WindDirectionDegrees), nameof(WeatherObservation.PressureHpa), nameof(WeatherObservation.SoilTemperatureC), nameof(WeatherObservation.SoilMoisturePercent) })
            weather.Property(property).HasPrecision(10, 3);

        weather.HasIndex(x => new { x.TenantId, x.FeedId, x.ProviderObservationId }).IsUnique();

        weather.HasIndex(x => new { x.TenantId, x.FieldId, x.ObservedAt });

        weather.HasIndex(x => new { x.TenantId, x.LocationId, x.ObservedAt });

        weather.HasOne<WeatherLocation>().WithMany().HasForeignKey(x => new { x.TenantId, x.LocationId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        weather.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        weather.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        weather.HasOne<IntegrationFeed>().WithMany().HasForeignKey(x => new { x.TenantId, x.FeedId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        weather.HasOne<IngestionRecord>().WithMany().HasForeignKey(x => new { x.TenantId, x.IngestionRecordId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
