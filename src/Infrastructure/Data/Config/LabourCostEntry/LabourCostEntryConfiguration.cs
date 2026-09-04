using EdgIQ.MurimiOS.Domain.LabourCostEntry;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LabourCostEntry;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using LabourCostEntry = global::EdgIQ.MurimiOS.Domain.LabourCostEntry.LabourCostEntry;
internal static class LabourCostEntryConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var labour = modelBuilder.Entity<LabourCostEntry>();

        labour.ToTable("labour_entries", "finance");

        context.ConfigureTenantEntity(labour);

        labour.Property(x => x.WorkerReference).HasMaxLength(160);

        labour.Property(x => x.DurationMinutes).HasPrecision(18, 4);

        labour.Property(x => x.EnteredDurationHours).HasPrecision(18, 4);

        labour.Property(x => x.HourlyRate).HasPrecision(18, 4);

        labour.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        labour.Property(x => x.TotalCost).HasPrecision(18, 2);

        labour.Property(x => x.Notes).HasMaxLength(2_000);

        labour.HasIndex(x => new { x.TenantId, x.TransactionId }).IsUnique();

        labour.HasIndex(x => new { x.TenantId, x.FarmId, x.OccurredAt });

        PlatformDbContext.ConfigureOperationalCostRelationships(labour);

    }
}
