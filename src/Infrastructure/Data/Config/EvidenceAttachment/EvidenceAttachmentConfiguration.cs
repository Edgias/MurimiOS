using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EvidenceAttachment;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
internal static class EvidenceAttachmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var evidence = modelBuilder.Entity<EvidenceAttachment>();

        evidence.ToTable("attachments", "evidence");

        context.ConfigureTenantEntity(evidence);

        evidence.Property(x => x.FileName).HasMaxLength(255);

        evidence.Property(x => x.ContentType).HasMaxLength(120);

        evidence.Property(x => x.Sha256).HasMaxLength(64);

        evidence.Property(x => x.ObjectKey).HasMaxLength(500);

        evidence.HasIndex(x => new { x.TenantId, x.CropZoneId, x.CapturedAt });

        evidence.HasIndex(x => new { x.TenantId, x.Sha256 });

        evidence.HasOne<CropZone>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
