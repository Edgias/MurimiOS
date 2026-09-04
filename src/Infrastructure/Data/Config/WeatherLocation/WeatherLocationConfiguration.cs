using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WeatherLocation;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;
internal static class WeatherLocationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var location = modelBuilder.Entity<WeatherLocation>();

        location.ToTable("weather_locations", "insights");

        context.ConfigureTenantEntity(location);

        location.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_weather_locations_tenant_id_id");

        location.Property(x => x.GridKey).HasMaxLength(120);

        location.Property(x => x.Latitude).HasPrecision(9, 6);

        location.Property(x => x.Longitude).HasPrecision(9, 6);

        location.Property(x => x.ElevationMetres).HasPrecision(8, 2);

        location.HasIndex(x => new { x.TenantId, x.FarmId, x.GridKey }).IsUnique().HasFilter("field_id IS NULL");

        location.HasIndex(x => new { x.TenantId, x.FieldId, x.GridKey }).IsUnique().HasFilter("field_id IS NOT NULL");

        location.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        location.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
