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
using WeatherForecast = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherForecast;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;
internal static class WeatherForecastConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var forecast = modelBuilder.Entity<WeatherForecast>();

        forecast.ToTable("weather_forecasts", "insights");

        context.ConfigureTenantEntity(forecast);

        forecast.Property(x => x.ProviderForecastId).HasMaxLength(240);

        foreach (string property in new[] { nameof(WeatherForecast.MinimumTemperatureC), nameof(WeatherForecast.MaximumTemperatureC), nameof(WeatherForecast.PrecipitationMm), nameof(WeatherForecast.PrecipitationProbabilityPercent), nameof(WeatherForecast.WindSpeedMps), nameof(WeatherForecast.RelativeHumidityPercent) })
            forecast.Property(property).HasPrecision(10, 3);

        forecast.HasIndex(x => new { x.TenantId, x.FeedId, x.ProviderForecastId }).IsUnique();

        forecast.HasIndex(x => new { x.TenantId, x.FieldId, x.ValidFrom, x.ValidTo });

        forecast.HasOne<WeatherLocation>().WithMany().HasForeignKey(x => new { x.TenantId, x.LocationId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        forecast.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        forecast.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        forecast.HasOne<IntegrationFeed>().WithMany().HasForeignKey(x => new { x.TenantId, x.FeedId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        forecast.HasOne<IngestionRecord>().WithMany().HasForeignKey(x => new { x.TenantId, x.IngestionRecordId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
