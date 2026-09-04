using EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsRetentionPolicy;
using AnalyticsRetentionPolicy = global::EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy.AnalyticsRetentionPolicy;
internal static class AnalyticsRetentionPolicyConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var retention = modelBuilder.Entity<AnalyticsRetentionPolicy>();

        retention.ToTable("retention_policies", "analytics"); 
context.ConfigureTenantEntity(retention);

        retention.Property(x => x.ConfiguredBy).HasMaxLength(240);

        retention.HasIndex(x => x.TenantId).IsUnique().HasFilter("is_active");

        retention.HasIndex(x => new { x.IsActive, x.NextRunAt });

    }
}
