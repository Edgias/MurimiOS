using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockFeedBatch;
using Supplier = global::EdgIQ.MurimiOS.Domain.Supplier.Supplier;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
internal static class LivestockFeedBatchConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var feedBatch = modelBuilder.Entity<LivestockFeedBatch>();

        feedBatch.ToTable("feed_batches", "livestock");

        context.ConfigureTenantEntity(feedBatch);

        feedBatch.Property(x => x.BatchCode).HasMaxLength(120);

        feedBatch.Property(x => x.FeedName).HasMaxLength(200);

        feedBatch.Property(x => x.InitialQuantity).HasPrecision(18, 4);

        feedBatch.Property(x => x.QuantityOnHand).HasPrecision(18, 4);

        feedBatch.Property(x => x.Unit).HasMaxLength(30);

        feedBatch.Property(x => x.EnteredInitialQuantity).HasPrecision(18, 4);

        feedBatch.Property(x => x.EnteredUnit).HasMaxLength(30);

        feedBatch.Property(x => x.DisplayUnit).HasMaxLength(30);

        feedBatch.Property(x => x.Supplier).HasMaxLength(200);

        feedBatch.HasIndex(x => new { x.TenantId, x.FarmId, x.BatchCode }).IsUnique();

        feedBatch.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
