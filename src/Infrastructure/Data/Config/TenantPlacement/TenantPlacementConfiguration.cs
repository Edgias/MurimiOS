using EdgIQ.MurimiOS.Domain.TenantPlacement;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantPlacement;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;
internal static class TenantPlacementConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var placement = modelBuilder.Entity<TenantPlacement>();

        placement.ToTable("tenant_placements", "enterprise"); 
context.ConfigureTenantEntity(placement);

        placement.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_tenant_placements_tenant_id_id");

        placement.Property(x => x.Region).HasMaxLength(80); 
placement.Property(x => x.DatabaseKey).HasMaxLength(160);

        placement.Property(x => x.ResidencyCountry).HasMaxLength(2).IsFixedLength();

        placement.HasIndex(x => x.TenantId).IsUnique().HasFilter("is_primary AND effective_to IS NULL");

    }
}
