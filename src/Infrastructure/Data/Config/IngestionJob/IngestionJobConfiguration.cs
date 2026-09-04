using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IngestionJob;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
internal static class IngestionJobConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var job = modelBuilder.Entity<IngestionJob>();

        job.ToTable("ingestion_jobs", "integrations");

        context.ConfigureTenantEntity(job);

        job.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_ingestion_jobs_tenant_id_id");

        job.Property(x => x.IdempotencyKey).HasMaxLength(160);

        job.Property(x => x.RequestedBy).HasMaxLength(240);

        job.Property(x => x.ResultCheckpointJson).HasColumnType("jsonb");

        job.Property(x => x.LastErrorCode).HasMaxLength(80);

        job.Property(x => x.LastErrorMessage).HasMaxLength(2_000);

        job.HasIndex(x => new { x.TenantId, x.FeedId, x.IdempotencyKey }).IsUnique();

        job.HasIndex(x => new { x.TenantId, x.Status, x.AvailableAt, x.LeasedUntil });

        job.HasIndex(x => new { x.TenantId, x.FarmId, x.RequestedAt });

        job.HasOne<IntegrationFeed>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FeedId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        job.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        job.HasOne<IngestionJob>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PreviousJobId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
