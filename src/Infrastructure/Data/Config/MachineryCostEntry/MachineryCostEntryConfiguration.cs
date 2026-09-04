using EdgIQ.MurimiOS.Domain.MachineryCostEntry;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.MachineryCostEntry;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using MachineryCostEntry = global::EdgIQ.MurimiOS.Domain.MachineryCostEntry.MachineryCostEntry;
internal static class MachineryCostEntryConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var machinery = modelBuilder.Entity<MachineryCostEntry>();

        machinery.ToTable("machinery_entries", "finance");

        context.ConfigureTenantEntity(machinery);

        machinery.Property(x => x.MachineReference).HasMaxLength(160);

        machinery.Property(x => x.DurationMinutes).HasPrecision(18, 4);

        machinery.Property(x => x.EnteredDurationHours).HasPrecision(18, 4);

        machinery.Property(x => x.HourlyRate).HasPrecision(18, 4);

        machinery.Property(x => x.AdditionalCost).HasPrecision(18, 2);

        machinery.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        machinery.Property(x => x.TotalCost).HasPrecision(18, 2);

        machinery.Property(x => x.Notes).HasMaxLength(2_000);

        machinery.HasIndex(x => new { x.TenantId, x.TransactionId }).IsUnique();

        machinery.HasIndex(x => new { x.TenantId, x.FarmId, x.OccurredAt });

        PlatformDbContext.ConfigureOperationalCostRelationships(machinery);

    }
}
