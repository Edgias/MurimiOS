using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.UsageMeterPeriod;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;
internal static class UsageMeterPeriodConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var usagePeriod = modelBuilder.Entity<UsageMeterPeriod>();

        usagePeriod.ToTable("usage_periods", "saas"); 
context.ConfigureTenantEntity(usagePeriod);

        usagePeriod.Property(x => x.MetricCode).HasMaxLength(120);

        usagePeriod.Property(x => x.Unit).HasMaxLength(40);

        usagePeriod.Property(x => x.Quantity).HasPrecision(20, 4);

        usagePeriod.HasIndex(x => new { x.TenantId, x.MetricCode, x.PeriodStartsAt, x.PeriodEndsAt }).IsUnique();

    }
}
