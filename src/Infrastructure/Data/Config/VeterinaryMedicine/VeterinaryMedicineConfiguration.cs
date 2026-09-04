using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.VeterinaryMedicine;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;
internal static class VeterinaryMedicineConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var medicine = modelBuilder.Entity<VeterinaryMedicine>();

        medicine.ToTable("medicines", "livestock");

        context.ConfigureTenantEntity(medicine);

        medicine.Property(x => x.Name).HasMaxLength(160);

        medicine.Property(x => x.ActiveIngredient).HasMaxLength(160);

        medicine.Property(x => x.Unit).HasMaxLength(30);

        medicine.Property(x => x.DisplayUnit).HasMaxLength(30);

        medicine.HasIndex(x => new { x.TenantId, x.FarmId, x.Name }).IsUnique();

        medicine.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
