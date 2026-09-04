using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EpcisExchange;
using EpcisCapturedEvent = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisCapturedEvent;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class EpcisCapturedEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var epcisEvent = modelBuilder.Entity<EpcisCapturedEvent>();

        epcisEvent.ToTable("epcis_captured_events", "traceability");

        context.ConfigureTenantEntity(epcisEvent);

        epcisEvent.Property(x => x.EventId).HasMaxLength(500);

        epcisEvent.Property(x => x.EventType).HasMaxLength(40);

        epcisEvent.Property(x => x.BusinessStep).HasMaxLength(500);

        epcisEvent.Property(x => x.Disposition).HasMaxLength(500);

        epcisEvent.Property(x => x.ObjectIdentifiersJson).HasColumnType("jsonb");

        epcisEvent.Property(x => x.Sha256).HasMaxLength(64).IsFixedLength();

        epcisEvent.Property(x => x.PayloadJson).HasColumnType("jsonb");

        epcisEvent.HasIndex(x => new { x.TenantId, x.EventId }).IsUnique();

        epcisEvent.HasIndex(x => new { x.TenantId, x.FarmId, x.EventTime });

        epcisEvent.HasIndex(x => new { x.TenantId, x.ExchangeId });

        epcisEvent.HasOne<EpcisExchange>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ExchangeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        epcisEvent.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
