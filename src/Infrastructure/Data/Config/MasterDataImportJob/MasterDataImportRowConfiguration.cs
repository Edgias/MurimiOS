using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.MasterDataImportJob;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
using MasterDataImportRow = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportRow;
internal static class MasterDataImportRowConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var masterRow = modelBuilder.Entity<MasterDataImportRow>();

        masterRow.ToTable("master_data_rows", "imports");

        context.ConfigureTenantEntity(masterRow);

        masterRow.Property(x => x.RawJson).HasColumnType("jsonb");

        masterRow.Property(x => x.NormalizedJson).HasColumnType("jsonb");

        masterRow.Property(x => x.IssuesJson).HasColumnType("jsonb");

        masterRow.Property(x => x.AppliedEntityType).HasMaxLength(160);

        masterRow.Property(x => x.FailureCode).HasMaxLength(80);

        masterRow.Property(x => x.FailureDetail).HasMaxLength(2_000);

        masterRow.HasIndex(x => new { x.TenantId, x.JobId, x.Ordinal }).IsUnique();

        masterRow.HasIndex(x => new { x.TenantId, x.JobId, x.Status, x.Ordinal });

        masterRow.HasOne<MasterDataImportJob>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.JobId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Cascade);

        masterRow.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AppliedFarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
