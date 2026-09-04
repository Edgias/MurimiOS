using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantRegionalSettings;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;

using TenantRegionalSettings = global::EdgIQ.MurimiOS.Domain.TenantRegionalSettings.TenantRegionalSettings;
internal static class TenantRegionalSettingsConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var settings = modelBuilder.Entity<TenantRegionalSettings>();

        settings.ToTable("tenant_regional_settings", "configuration");

        context.ConfigureTenantEntity(settings);

        settings.Property(x => x.CountryCode).HasMaxLength(2);

        settings.Property(x => x.CurrencyCode).HasMaxLength(3);

        settings.Property(x => x.Locale).HasMaxLength(40);

        settings.Property(x => x.TimeZone).HasMaxLength(100);

        settings.Property(x => x.AreaUnit).HasMaxLength(20);

        settings.Property(x => x.MassUnit).HasMaxLength(20);

        settings.Property(x => x.VolumeUnit).HasMaxLength(20);

        settings.Property(x => x.TemperatureUnit).HasMaxLength(20);

        settings.HasIndex(x => x.TenantId).IsUnique();

    }
}
