using EdgIQ.MurimiOS.Domain.MediaUploadSession;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.MediaUploadSession;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using MediaUploadSession = global::EdgIQ.MurimiOS.Domain.MediaUploadSession.MediaUploadSession;
internal static class MediaUploadSessionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var upload = modelBuilder.Entity<MediaUploadSession>();

        upload.ToTable("upload_sessions", "evidence");

        context.ConfigureTenantEntity(upload);

        upload.Property(x => x.FileName).HasMaxLength(255);

        upload.Property(x => x.ContentType).HasMaxLength(120);

        upload.Property(x => x.ExpectedSha256).HasMaxLength(64);

        upload.HasIndex(x => new { x.TenantId, x.EvidenceId }).IsUnique();

        upload.HasIndex(x => new { x.TenantId, x.Status, x.ExpiresAt });

        upload.HasOne<CropZone>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
