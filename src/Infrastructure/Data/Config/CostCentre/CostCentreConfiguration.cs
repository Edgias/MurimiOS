using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.CostCentre;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CostCentre;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using CostCentre = global::EdgIQ.MurimiOS.Domain.CostCentre.CostCentre;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class CostCentreConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var centre = modelBuilder.Entity<CostCentre>();

        centre.ToTable("cost_centres", "finance");

        context.ConfigureTenantEntity(centre);

        centre.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_finance_cost_centres_tenant_id_id");

        centre.Property(x => x.Code).HasMaxLength(80);

        centre.Property(x => x.Name).HasMaxLength(200);

        centre.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        centre.Property(x => x.ClosedBy).HasMaxLength(240);

        centre.HasIndex(x => new { x.TenantId, x.FarmId, x.Code }).IsUnique();

        centre.HasIndex(x => new { x.TenantId, x.Type, x.TargetId });

        centre.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
