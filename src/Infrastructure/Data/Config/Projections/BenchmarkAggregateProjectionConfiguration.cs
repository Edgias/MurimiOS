using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Projections;
using BenchmarkAggregateProjection = global::EdgIQ.MurimiOS.Application.Common.Abstractions.BenchmarkAggregateProjection;
internal static class BenchmarkAggregateProjectionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var benchmark = modelBuilder.Entity<BenchmarkAggregateProjection>();

        benchmark.HasNoKey().ToView(null);

        benchmark.Property(x => x.Minimum).HasPrecision(28, 8);

        benchmark.Property(x => x.Percentile25).HasPrecision(28, 8);

        benchmark.Property(x => x.Median).HasPrecision(28, 8);

        benchmark.Property(x => x.Percentile75).HasPrecision(28, 8);

        benchmark.Property(x => x.Maximum).HasPrecision(28, 8);

        benchmark.Property(x => x.Average).HasPrecision(28, 8);

        benchmark.Property(x => x.AverageDataQualityPercent).HasPrecision(8, 4);

    }
}
