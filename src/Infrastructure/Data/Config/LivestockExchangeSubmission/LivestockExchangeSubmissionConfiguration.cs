using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockExchangeSubmission;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockExchangeSubmission = global::EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission.LivestockExchangeSubmission;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockExchangeSubmissionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var exchangeSubmission = modelBuilder.Entity<LivestockExchangeSubmission>();

        exchangeSubmission.ToTable("exchange_submissions", "livestock");

        context.ConfigureTenantEntity(exchangeSubmission);

        exchangeSubmission.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_livestock_exchange_submissions_tenant_id_id");

        exchangeSubmission.Property(x => x.ProviderCode).HasMaxLength(80);

        exchangeSubmission.Property(x => x.SchemaVersion).HasMaxLength(40);

        exchangeSubmission.Property(x => x.MediaType).HasMaxLength(120);

        exchangeSubmission.Property(x => x.PayloadJson).HasColumnType("jsonb");

        exchangeSubmission.Property(x => x.PayloadSha256).HasMaxLength(64).IsFixedLength();

        exchangeSubmission.Property(x => x.CreatedBy).HasMaxLength(240);

        exchangeSubmission.Property(x => x.ExternalReference).HasMaxLength(160);

        exchangeSubmission.Property(x => x.RejectionCode).HasMaxLength(80);

        exchangeSubmission.Property(x => x.RejectionMessage).HasMaxLength(2_000);

        exchangeSubmission.HasIndex(x => new { x.TenantId, x.ExchangeType, x.SourceRecordId, x.ProviderCode }).IsUnique();

        exchangeSubmission.HasIndex(x => new { x.TenantId, x.Status, x.ReportingDueAt });

        exchangeSubmission.HasOne<LivestockRegulatoryProfile>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ProfileId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        exchangeSubmission.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        exchangeSubmission.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
