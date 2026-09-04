using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.UsageMeterPeriod;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.UsageMeterPeriod;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;

using UsageMeterEvent = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterEvent;
internal static class UsageMeterEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var usageEvent = modelBuilder.Entity<UsageMeterEvent>();

        usageEvent.ToTable("usage_events", "saas"); 
context.ConfigureTenantEntity(usageEvent);

        usageEvent.Property(x => x.IdempotencyKey).HasMaxLength(160);

        usageEvent.Property(x => x.MetricCode).HasMaxLength(120);

        usageEvent.Property(x => x.Quantity).HasPrecision(20, 4);

        usageEvent.Property(x => x.Unit).HasMaxLength(40);

        usageEvent.Property(x => x.Source).HasMaxLength(120);

        usageEvent.Property(x => x.DimensionsJson).HasColumnType("jsonb");

        usageEvent.HasIndex(x => new { x.TenantId, x.IdempotencyKey }).IsUnique();

        usageEvent.HasIndex(x => new { x.TenantId, x.MetricCode, x.OccurredAt });

    }
}
