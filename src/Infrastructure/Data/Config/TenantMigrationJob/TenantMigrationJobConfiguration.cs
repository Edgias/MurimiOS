using EdgIQ.MurimiOS.Domain.TenantPlacement;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantMigrationJob;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;
internal static class TenantMigrationJobConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var migration = modelBuilder.Entity<TenantMigrationJob>();

        migration.ToTable("tenant_migration_jobs", "enterprise"); 
context.ConfigureTenantEntity(migration);

        migration.Property(x => x.DestinationRegion).HasMaxLength(80); 
migration.Property(x => x.DestinationDatabaseKey).HasMaxLength(160);

        migration.Property(x => x.RequestedBy).HasMaxLength(240); 
migration.Property(x => x.ArtifactObjectKey).HasMaxLength(1000);

        migration.Property(x => x.ArtifactSha256).HasMaxLength(64).IsFixedLength(); 
migration.Property(x => x.ManifestJson).HasColumnType("jsonb");

        migration.Property(x => x.FailureCode).HasMaxLength(80); 
migration.Property(x => x.FailureMessage).HasMaxLength(2000);

        migration.HasIndex(x => new { x.TenantId, x.Status, x.RequestedAt });

        migration.HasOne<TenantPlacement>().WithMany().HasForeignKey(x => new { x.TenantId, x.SourcePlacementId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
