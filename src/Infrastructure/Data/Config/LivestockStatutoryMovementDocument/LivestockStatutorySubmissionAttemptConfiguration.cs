using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockStatutoryMovementDocument;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockStatutorySubmissionAttempt = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutorySubmissionAttempt;
internal static class LivestockStatutorySubmissionAttemptConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var statutoryAttempt = modelBuilder.Entity<LivestockStatutorySubmissionAttempt>();

        statutoryAttempt.ToTable("statutory_submission_attempts", "livestock");

        context.ConfigureTenantEntity(statutoryAttempt);

        statutoryAttempt.Property(x => x.RequestSha256).HasMaxLength(64).IsFixedLength();

        statutoryAttempt.Property(x => x.ExternalReference).HasMaxLength(160);

        statutoryAttempt.Property(x => x.ResponseJson).HasColumnType("jsonb");

        statutoryAttempt.Property(x => x.ErrorCode).HasMaxLength(80);

        statutoryAttempt.Property(x => x.ErrorMessage).HasMaxLength(2_000);

        statutoryAttempt.HasIndex(x => new { x.TenantId, x.DocumentId, x.AttemptNumber }).IsUnique();

        statutoryAttempt.HasOne<LivestockStatutoryMovementDocument>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.DocumentId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
