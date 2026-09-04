using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.DerivedMediaAsset;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
internal static class DerivedMediaAssetConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var derivedMedia = modelBuilder.Entity<DerivedMediaAsset>();

        derivedMedia.ToTable("derived_media_assets", "connected");

        context.ConfigureTenantEntity(derivedMedia);

        derivedMedia.Property(x => x.SourceType).HasMaxLength(100);

        derivedMedia.Property(x => x.ObjectKey).HasMaxLength(1_024);

        derivedMedia.Property(x => x.ContentType).HasMaxLength(160);

        derivedMedia.Property(x => x.Sha256).HasMaxLength(64).IsFixedLength();

        derivedMedia.Property(x => x.LegalHoldReason).HasMaxLength(1_000);

        derivedMedia.Property(x => x.LegalHoldChangedBy).HasMaxLength(240);

        derivedMedia.Property(x => x.LastDeletionError).HasMaxLength(2_000);

        derivedMedia.HasIndex(x => new { x.TenantId, x.ObjectKey }).IsUnique();

        derivedMedia.HasIndex(x => new { x.TenantId, x.SourceType, x.SourceId, x.Kind }).IsUnique();

        derivedMedia.HasIndex(x => new { x.Status, x.LegalHold, x.GeneratedAt });

        derivedMedia.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        derivedMedia.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
