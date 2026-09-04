using EdgIQ.MurimiOS.Domain.KpiDefinition;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.KpiDefinition;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;
internal static class KpiDefinitionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var definition = modelBuilder.Entity<KpiDefinition>();

        definition.ToTable("kpi_definitions", "analytics");

        context.ConfigureTenantEntity(definition);

        definition.Property(x => x.Code).HasMaxLength(120);

        definition.Property(x => x.Name).HasMaxLength(200);

        definition.Property(x => x.Description).HasMaxLength(2_000);

        definition.Property(x => x.Formula).HasMaxLength(4_000);

        definition.Property(x => x.Unit).HasMaxLength(40);

        definition.Property(x => x.TimeGrain).HasMaxLength(40);

        definition.Property(x => x.Owner).HasMaxLength(240);

        definition.Property(x => x.Exclusions).HasMaxLength(2_000);

        definition.Property(x => x.QualityRule).HasMaxLength(2_000);

        definition.HasIndex(x => new { x.TenantId, x.SeriesId, x.DefinitionVersion }).IsUnique();

        definition.HasIndex(x => new { x.TenantId, x.Code, x.Status });

    }
}
