using EdgIQ.MurimiOS.Domain.FarmField;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FarmField;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
internal static class FieldBoundaryVersionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var boundary = modelBuilder.Entity<FieldBoundaryVersion>();

        boundary.ToTable("field_boundary_versions", "farm");

        context.ConfigureTenantEntity(boundary);

        boundary.Property(x => x.Boundary).HasColumnType("geography (multipolygon, 4326)");

        boundary.Property(x => x.AreaHa)
            .HasPrecision(14, 4)
            .HasComputedColumnSql("ST_Area(boundary) / 10000.0", stored: true);

        boundary.Property(x => x.Source).HasMaxLength(40);

        boundary.HasIndex(x => new { x.TenantId, x.FieldId, x.ValidFrom });

        boundary.HasOne<FarmField>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
