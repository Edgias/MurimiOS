using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockHolding;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
internal static class LivestockHoldingConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var holding = modelBuilder.Entity<LivestockHolding>();

        holding.ToTable("holdings", "livestock");

        context.ConfigureTenantEntity(holding);

        holding.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_holdings_tenant_id_id");

        holding.Property(x => x.Code).HasMaxLength(80);

        holding.Property(x => x.Name).HasMaxLength(200);

        holding.HasIndex(x => new { x.TenantId, x.FarmId, x.Code }).IsUnique();

        holding.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
