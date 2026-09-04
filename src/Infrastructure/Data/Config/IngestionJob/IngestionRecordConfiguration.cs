using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IngestionJob;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
internal static class IngestionRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var record = modelBuilder.Entity<IngestionRecord>();

        record.ToTable("ingestion_records", "integrations");

        context.ConfigureTenantEntity(record);

        record.Property(x => x.ExternalRecordId).HasMaxLength(240);

        record.Property(x => x.ContentSha256).HasMaxLength(64).IsFixedLength();

        record.Property(x => x.PayloadJson).HasColumnType("jsonb");

        record.Property(x => x.RejectionCode).HasMaxLength(80);

        record.Property(x => x.RejectionReason).HasMaxLength(2_000);

        record.HasIndex(x => new { x.TenantId, x.FeedId, x.ExternalRecordId, x.ContentSha256 }).IsUnique();

        record.HasIndex(x => new { x.TenantId, x.FarmId, x.ObservedAt });

        record.HasIndex(x => new { x.TenantId, x.JobId, x.Status });

        record.HasOne<IngestionJob>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.JobId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        record.HasOne<IntegrationFeed>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FeedId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        record.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
