using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.LivestockTreatment;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockTreatment;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
using LivestockTreatment = global::EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using VeterinaryMedicineStockLot = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot.VeterinaryMedicineStockLot;
internal static class LivestockTreatmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var treatment = modelBuilder.Entity<LivestockTreatment>();

        treatment.ToTable("treatments", "livestock");

        context.ConfigureTenantEntity(treatment);

        treatment.Property(x => x.Dose).HasPrecision(18, 4);

        treatment.Property(x => x.Unit).HasMaxLength(30);

        treatment.Property(x => x.EnteredDose).HasPrecision(18, 4);

        treatment.Property(x => x.EnteredUnit).HasMaxLength(30);

        treatment.Property(x => x.DisplayUnit).HasMaxLength(30);

        treatment.Property(x => x.Route).HasMaxLength(80);

        treatment.Property(x => x.Notes).HasMaxLength(2_000);

        treatment.HasIndex(x => new { x.TenantId, x.SubjectId, x.AdministeredAt });

        treatment.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique();

        treatment.HasOne<LivestockSubject>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        treatment.HasOne<VeterinaryMedicineStockLot>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.MedicineStockLotId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
