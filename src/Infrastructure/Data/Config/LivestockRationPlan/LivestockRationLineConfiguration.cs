using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockRationPlan;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockRationLine = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationLine;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
internal static class LivestockRationLineConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var rationLine = modelBuilder.Entity<LivestockRationLine>();

        rationLine.ToTable("ration_lines", "livestock");

        context.ConfigureTenantEntity(rationLine);

        rationLine.Property(x => x.FeedName).HasMaxLength(200);

        rationLine.Property(x => x.QuantityKgPerHeadPerDay).HasPrecision(18, 4);

        rationLine.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        rationLine.Property(x => x.EnteredUnit).HasMaxLength(30);

        rationLine.Property(x => x.DryMatterPercent).HasPrecision(7, 4);

        rationLine.HasIndex(x => new { x.TenantId, x.PlanId, x.FeedName });

        rationLine.HasOne<LivestockRationPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Cascade);

        rationLine.HasOne<LivestockFeedBatch>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FeedBatchId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
