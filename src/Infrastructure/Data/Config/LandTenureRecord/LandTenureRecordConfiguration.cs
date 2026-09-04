using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LandTenureRecord;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LandTenureRecord;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using LandTenureRecord = global::EdgIQ.MurimiOS.Domain.LandTenureRecord.LandTenureRecord;
internal static class LandTenureRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var tenure = modelBuilder.Entity<LandTenureRecord>();

        tenure.ToTable("land_tenure_records", "farm");

        context.ConfigureTenantEntity(tenure);

        tenure.Property(x => x.HolderName).HasMaxLength(240);

        tenure.Property(x => x.CounterpartyName).HasMaxLength(240);

        tenure.Property(x => x.Reference).HasMaxLength(160);

        tenure.Property(x => x.RegisteredAreaHa).HasPrecision(14, 4);

        tenure.Property(x => x.DocumentReference).HasMaxLength(500);

        tenure.Property(x => x.Notes).HasMaxLength(2_000);

        tenure.Property(x => x.ClosedBy).HasMaxLength(240);

        tenure.HasIndex(x => new { x.TenantId, x.Reference }).IsUnique();

        tenure.HasIndex(x => new { x.TenantId, x.FarmId, x.FieldId, x.ValidFrom, x.ValidTo });

        tenure.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        tenure.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
