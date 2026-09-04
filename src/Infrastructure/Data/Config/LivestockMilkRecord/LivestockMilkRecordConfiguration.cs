using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockMilkRecord;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockMilkRecord = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.LivestockMilkRecord;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockMilkRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var milk = modelBuilder.Entity<LivestockMilkRecord>();

        milk.ToTable("milk_records", "livestock");

        context.ConfigureTenantEntity(milk);

        milk.Property(x => x.QuantityLitres).HasPrecision(18, 4);

        milk.Property(x => x.DiscardedLitres).HasPrecision(18, 4);

        milk.Property(x => x.SaleableLitres).HasPrecision(18, 4);

        milk.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        milk.Property(x => x.EnteredDiscardedQuantity).HasPrecision(18, 4);

        milk.Property(x => x.EnteredUnit).HasMaxLength(30);

        milk.Property(x => x.FatPercent).HasPrecision(7, 4);

        milk.Property(x => x.ProteinPercent).HasPrecision(7, 4);

        milk.Property(x => x.QualityGrade).HasMaxLength(80);

        milk.Property(x => x.Notes).HasMaxLength(2_000);

        milk.HasIndex(x => new { x.TenantId, x.SubjectId, x.MilkedAt });

        milk.HasIndex(x => new { x.TenantId, x.FarmId, x.MilkedAt });

        milk.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        milk.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
