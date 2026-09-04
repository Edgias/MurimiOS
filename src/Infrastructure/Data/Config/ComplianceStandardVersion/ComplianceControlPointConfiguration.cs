using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceStandardVersion;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
internal static class ComplianceControlPointConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var control = modelBuilder.Entity<ComplianceControlPoint>();

        control.ToTable("control_points", "compliance");

        context.ConfigureTenantEntity(control);

        control.Property(x => x.Code).HasMaxLength(80);

        control.Property(x => x.Title).HasMaxLength(240);

        control.Property(x => x.Description).HasMaxLength(4_000);

        control.Property(x => x.Weight).HasPrecision(8, 4);

        control.Property(x => x.EvaluatorKey).HasMaxLength(120);

        control.Property(x => x.EvaluatorVersion).HasMaxLength(60);

        control.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_control_points_tenant_id_id");

        control.HasIndex(x => new { x.TenantId, x.StandardVersionId, x.Code }).IsUnique();

        control.HasIndex(x => new { x.TenantId, x.StandardVersionId, x.SortOrder });

        control.HasOne<ComplianceStandardVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.StandardVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
