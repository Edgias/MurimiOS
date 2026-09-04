using EdgIQ.MurimiOS.Domain.SoilSample;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilSample;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilSampleResult = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSampleResult;
internal static class SoilSampleResultConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var soilResult = modelBuilder.Entity<SoilSampleResult>();

        soilResult.ToTable("soil_sample_results", "farm");

        context.ConfigureTenantEntity(soilResult);

        soilResult.Property(x => x.AnalyteCode).HasMaxLength(80);

        soilResult.Property(x => x.AnalyteName).HasMaxLength(160);

        soilResult.Property(x => x.Value).HasPrecision(18, 6);

        soilResult.Property(x => x.Unit).HasMaxLength(40);

        soilResult.Property(x => x.Method).HasMaxLength(160);

        soilResult.Property(x => x.DetectionLimit).HasPrecision(18, 6);

        soilResult.Property(x => x.ResultFlag).HasMaxLength(40);

        soilResult.Property(x => x.RecordedBy).HasMaxLength(240);

        soilResult.HasIndex(x => new { x.TenantId, x.SampleId, x.AnalyteCode }).IsUnique();

        soilResult.HasOne<SoilSample>().WithMany().HasForeignKey(x => new { x.TenantId, x.SampleId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
