using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockExchangeSubmission;
using LivestockExchangeAttempt = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeAttempt;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
internal static class LivestockExchangeAttemptConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var exchangeAttempt = modelBuilder.Entity<LivestockExchangeAttempt>();

        exchangeAttempt.ToTable("exchange_attempts", "livestock");

        context.ConfigureTenantEntity(exchangeAttempt);

        exchangeAttempt.Property(x => x.RequestSha256).HasMaxLength(64).IsFixedLength();

        exchangeAttempt.Property(x => x.ExternalReference).HasMaxLength(160);

        exchangeAttempt.Property(x => x.ResponseJson).HasColumnType("jsonb");

        exchangeAttempt.Property(x => x.ErrorCode).HasMaxLength(80);

        exchangeAttempt.Property(x => x.ErrorMessage).HasMaxLength(2_000);

        exchangeAttempt.HasIndex(x => new { x.TenantId, x.SubmissionId, x.AttemptNumber }).IsUnique();

        exchangeAttempt.HasOne<LivestockExchangeSubmission>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubmissionId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
