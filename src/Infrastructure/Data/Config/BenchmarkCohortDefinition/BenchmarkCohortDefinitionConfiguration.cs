using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.BenchmarkCohortDefinition;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
internal static class BenchmarkCohortDefinitionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var cohort = modelBuilder.Entity<BenchmarkCohortDefinition>();

        cohort.ToTable("benchmark_cohort_definitions", "analytics"); 
context.ConfigureTenantEntity(cohort);

        cohort.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_benchmark_cohort_definitions_tenant_id_id");

        cohort.Property(x => x.CohortKey).HasMaxLength(120); 
cohort.Property(x => x.Name).HasMaxLength(200);

        cohort.Property(x => x.Description).HasMaxLength(2_000); 
cohort.Property(x => x.InclusionCriteriaJson).HasColumnType("jsonb");

        cohort.Property(x => x.Owner).HasMaxLength(240); 
cohort.Property(x => x.CreatedBy).HasMaxLength(240);

        cohort.Property(x => x.ApprovedBy).HasMaxLength(240); 
cohort.Property(x => x.ApprovalReference).HasMaxLength(240);

        cohort.Property(x => x.RetiredBy).HasMaxLength(240);

        cohort.HasIndex(x => new { x.TenantId, x.CohortKey, x.DefinitionVersion }).IsUnique();

        cohort.HasIndex(x => new { x.TenantId, x.CohortKey, x.Status, x.EffectiveFrom });

        modelBuilder.Entity<EdgIQ.MurimiOS.Domain.BenchmarkEnrollment.BenchmarkEnrollment>().HasOne<BenchmarkCohortDefinition>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CohortDefinitionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
