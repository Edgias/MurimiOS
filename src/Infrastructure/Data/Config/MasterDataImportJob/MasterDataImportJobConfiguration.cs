using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.MasterDataImportJob;
using MasterDataImportJob = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportJob;
internal static class MasterDataImportJobConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var masterJob = modelBuilder.Entity<MasterDataImportJob>();

        masterJob.ToTable("master_data_jobs", "imports");

        context.ConfigureTenantEntity(masterJob);

        masterJob.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_master_data_jobs_tenant_id_id");

        masterJob.Property(x => x.FileName).HasMaxLength(255);

        masterJob.Property(x => x.Sha256).HasMaxLength(64).IsFixedLength();

        masterJob.Property(x => x.ObjectKey).HasMaxLength(500);

        masterJob.Property(x => x.ParserVersion).HasMaxLength(40);

        masterJob.Property(x => x.HeaderJson).HasColumnType("jsonb");

        masterJob.Property(x => x.AppliedBy).HasMaxLength(240);

        masterJob.Property(x => x.FailureCode).HasMaxLength(80);

        masterJob.Property(x => x.FailureDetail).HasMaxLength(2_000);

        masterJob.HasIndex(x => new { x.TenantId, x.Type, x.Sha256 }).IsUnique();

        masterJob.HasIndex(x => new { x.TenantId, x.Status, x.CreatedAtUtc });

    }
}
