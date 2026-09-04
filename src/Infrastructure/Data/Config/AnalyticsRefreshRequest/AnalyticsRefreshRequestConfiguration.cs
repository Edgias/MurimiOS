using EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsRefreshRequest;
using AnalyticsRefreshRequest = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequest;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class AnalyticsRefreshRequestConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var refresh = modelBuilder.Entity<AnalyticsRefreshRequest>();

        refresh.ToTable("refresh_requests", "analytics"); 
context.ConfigureTenantEntity(refresh);

        refresh.Property(x => x.TriggerType).HasMaxLength(200); 
refresh.Property(x => x.LastError).HasMaxLength(2_000);

        refresh.HasIndex(x => new { x.TenantId, x.Status, x.AvailableAt });

        refresh.HasIndex(x => new { x.TenantId, x.FarmId }).IsUnique().HasFilter("status = 1");

        refresh.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
