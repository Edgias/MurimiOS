using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EpcisExchange;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class EpcisExchangeConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var epcisExchange = modelBuilder.Entity<EpcisExchange>();

        epcisExchange.ToTable("epcis_exchanges", "traceability");

        context.ConfigureTenantEntity(epcisExchange);

        epcisExchange.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_epcis_exchanges_tenant_id_id");

        epcisExchange.Property(x => x.DocumentType).HasMaxLength(40);

        epcisExchange.Property(x => x.DocumentId).HasMaxLength(500);

        epcisExchange.Property(x => x.SchemaVersion).HasMaxLength(20);

        epcisExchange.Property(x => x.CbvVersion).HasMaxLength(20);

        epcisExchange.Property(x => x.Sha256).HasMaxLength(64).IsFixedLength();

        epcisExchange.Property(x => x.Sender).HasMaxLength(500);

        epcisExchange.Property(x => x.Receiver).HasMaxLength(500);

        epcisExchange.Property(x => x.PayloadJson).HasColumnType("jsonb");

        epcisExchange.Property(x => x.ExchangedBy).HasMaxLength(240);

        epcisExchange.HasIndex(x => new { x.TenantId, x.Direction, x.Sha256 }).IsUnique();

        epcisExchange.HasIndex(x => new { x.TenantId, x.FarmId, x.ExchangedAt });

        epcisExchange.HasIndex(x => new { x.TenantId, x.DocumentId }).IsUnique();

        epcisExchange.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
