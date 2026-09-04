using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ReferenceCatalogue;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
internal static class ReferenceCatalogueConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var catalogue = modelBuilder.Entity<ReferenceCatalogue>();

        catalogue.ToTable("reference_catalogues", "configuration");

        context.ConfigureTenantEntity(catalogue);

        catalogue.HasAlternateKey(x => new { x.TenantId, x.Id })
            .HasName("ak_reference_catalogues_tenant_id_id");

        catalogue.Property(x => x.Key).HasMaxLength(120);

        catalogue.Property(x => x.Name).HasMaxLength(200);

        catalogue.Property(x => x.CatalogueVersion).HasMaxLength(80);

        catalogue.Property(x => x.CountryCode).HasMaxLength(2);

        catalogue.Property(x => x.Authority).HasMaxLength(200);

        catalogue.HasIndex(x => new { x.TenantId, x.Key, x.CatalogueVersion }).IsUnique();

        catalogue.HasIndex(x => new { x.TenantId, x.CountryCode, x.EffectiveFrom, x.EffectiveTo });

    }
}
