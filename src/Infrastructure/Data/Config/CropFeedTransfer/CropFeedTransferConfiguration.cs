using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropFeedTransfer;
using CropFeedTransfer = global::EdgIQ.MurimiOS.Domain.CropFeedTransfer.CropFeedTransfer;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
internal static class CropFeedTransferConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var cropFeed = modelBuilder.Entity<CropFeedTransfer>();

        cropFeed.ToTable("crop_feed_transfers", "livestock");

        context.ConfigureTenantEntity(cropFeed);

        cropFeed.Property(x => x.Quantity).HasPrecision(18, 4);

        cropFeed.Property(x => x.Unit).HasMaxLength(30);

        cropFeed.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        cropFeed.Property(x => x.EnteredUnit).HasMaxLength(30);

        cropFeed.Property(x => x.TransferredBy).HasMaxLength(240);

        cropFeed.Property(x => x.Notes).HasMaxLength(2_000);

        cropFeed.HasIndex(x => new { x.TenantId, x.SourceType, x.SourceId, x.TransferredAt });

        cropFeed.HasIndex(x => new { x.TenantId, x.FeedBatchId }).IsUnique();

        cropFeed.HasIndex(x => new { x.TenantId, x.TraceEventId }).IsUnique();

        cropFeed.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        cropFeed.HasOne<LivestockFeedBatch>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FeedBatchId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        cropFeed.HasOne<TraceEvent>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.TraceEventId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
