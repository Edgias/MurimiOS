using EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.BenchmarkEnrollment;
using BenchmarkEnrollment = global::EdgIQ.MurimiOS.Domain.BenchmarkEnrollment.BenchmarkEnrollment;
internal static class BenchmarkEnrollmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var enrollment = modelBuilder.Entity<BenchmarkEnrollment>();

        enrollment.ToTable("benchmark_enrollments", "analytics");

        context.ConfigureTenantEntity(enrollment);

        enrollment.Property(x => x.MetricCode).HasMaxLength(120);

        enrollment.Property(x => x.CohortKey).HasMaxLength(120);

        enrollment.Property(x => x.EnrolledBy).HasMaxLength(240);

        enrollment.Property(x => x.RevokedBy).HasMaxLength(240);

        enrollment.HasIndex(x => new { x.TenantId, x.MetricCode, x.CohortKey }).IsUnique().HasFilter("is_active");

        enrollment.HasIndex(x => new { x.MetricCode, x.CohortKey, x.IsActive });

    }
}
