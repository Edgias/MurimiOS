using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IntegrationFeed;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
internal static class IntegrationFeedConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var feed = modelBuilder.Entity<IntegrationFeed>();

        feed.ToTable("feeds", "integrations");

        context.ConfigureTenantEntity(feed);

        feed.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_integration_feeds_tenant_id_id");

        feed.Property(x => x.Name).HasMaxLength(200);

        feed.Property(x => x.ExternalSourceId).HasMaxLength(240);

        feed.Property(x => x.ConfigurationJson).HasColumnType("jsonb");

        feed.Property(x => x.CheckpointJson).HasColumnType("jsonb");

        feed.Property(x => x.LastErrorCode).HasMaxLength(80);

        feed.Property(x => x.LastErrorMessage).HasMaxLength(2_000);

        feed.HasIndex(x => new { x.TenantId, x.ConnectionId, x.ExternalSourceId }).IsUnique();

        feed.HasIndex(x => new { x.TenantId, x.FarmId, x.FeedType, x.Status });

        feed.HasIndex(x => new { x.TenantId, x.Status, x.NextPollAt });

        feed.HasOne<IntegrationConnection>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ConnectionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        feed.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
