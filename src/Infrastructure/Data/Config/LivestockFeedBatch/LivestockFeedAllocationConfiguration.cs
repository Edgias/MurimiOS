using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockFeedBatch;
using LivestockFeedAllocation = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedAllocation;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockFeedAllocationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var feedAllocation = modelBuilder.Entity<LivestockFeedAllocation>();

        feedAllocation.ToTable("feed_allocations", "livestock");

        context.ConfigureTenantEntity(feedAllocation);

        feedAllocation.Property(x => x.Quantity).HasPrecision(18, 4);

        feedAllocation.Property(x => x.Unit).HasMaxLength(30);

        feedAllocation.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        feedAllocation.Property(x => x.EnteredUnit).HasMaxLength(30);

        feedAllocation.Property(x => x.DisplayUnit).HasMaxLength(30);

        feedAllocation.Property(x => x.Notes).HasMaxLength(2_000);

        feedAllocation.HasIndex(x => new { x.TenantId, x.SubjectId, x.OccurredAt });

        feedAllocation.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique().HasFilter("source_command_id IS NOT NULL");

        feedAllocation.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        feedAllocation.HasOne<LivestockFeedBatch>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FeedBatchId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
