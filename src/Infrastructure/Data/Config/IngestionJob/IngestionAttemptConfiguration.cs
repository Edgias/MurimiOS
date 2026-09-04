using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IngestionJob;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
internal static class IngestionAttemptConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var attempt = modelBuilder.Entity<IngestionAttempt>();

        attempt.ToTable("ingestion_attempts", "integrations");

        context.ConfigureTenantEntity(attempt);

        attempt.Property(x => x.Cost).HasPrecision(18, 6);

        attempt.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        attempt.Property(x => x.ErrorCode).HasMaxLength(80);

        attempt.Property(x => x.ErrorMessage).HasMaxLength(2_000);

        attempt.HasIndex(x => new { x.TenantId, x.JobId, x.AttemptNumber }).IsUnique();

        attempt.HasIndex(x => new { x.TenantId, x.CompletedAt });

        attempt.HasOne<IngestionJob>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.JobId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
