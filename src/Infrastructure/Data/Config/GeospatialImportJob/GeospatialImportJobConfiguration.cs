using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.GeospatialImportJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.GeospatialImportJob;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using GeospatialImportJob = global::EdgIQ.MurimiOS.Domain.GeospatialImportJob.GeospatialImportJob;
internal static class GeospatialImportJobConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var job = modelBuilder.Entity<GeospatialImportJob>();

        job.ToTable("geospatial_jobs", "imports");

        context.ConfigureTenantEntity(job);

        job.Property(x => x.FileName).HasMaxLength(255);

        job.Property(x => x.ContentType).HasMaxLength(120);

        job.Property(x => x.Sha256).HasMaxLength(64);

        job.Property(x => x.ObjectKey).HasMaxLength(500);

        job.Property(x => x.ParserVersion).HasMaxLength(40);

        job.Property(x => x.FailureCode).HasMaxLength(80);

        job.Property(x => x.FailureDetail).HasMaxLength(2_000);

        job.Property(x => x.AppliedBy).HasMaxLength(240);

        job.HasIndex(x => new { x.TenantId, x.FarmId, x.CreatedAtUtc });

        job.HasIndex(x => new { x.TenantId, x.Sha256, x.FarmId });

        job.HasIndex(x => new { x.TenantId, x.Status, x.CreatedAtUtc });

        job.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
