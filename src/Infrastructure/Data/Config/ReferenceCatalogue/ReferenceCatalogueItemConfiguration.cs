using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ReferenceCatalogue;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
internal static class ReferenceCatalogueItemConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var item = modelBuilder.Entity<ReferenceCatalogueItem>();

        item.ToTable("reference_catalogue_items", "configuration");

        context.ConfigureTenantEntity(item);

        item.Property(x => x.Code).HasMaxLength(120);

        item.Property(x => x.Name).HasMaxLength(200);

        item.Property(x => x.Description).HasMaxLength(2_000);

        item.Property(x => x.AttributesJson).HasColumnType("jsonb");

        item.HasIndex(x => new { x.TenantId, x.CatalogueId, x.Code }).IsUnique();

        item.HasIndex(x => new { x.TenantId, x.CatalogueId, x.EffectiveFrom, x.EffectiveTo });

        item.HasOne<ReferenceCatalogue>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CatalogueId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
