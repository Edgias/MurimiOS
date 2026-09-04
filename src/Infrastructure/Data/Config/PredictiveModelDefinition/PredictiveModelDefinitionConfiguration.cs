using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.PredictiveModelDefinition;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;
internal static class PredictiveModelDefinitionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var model = modelBuilder.Entity<PredictiveModelDefinition>();

        model.ToTable("predictive_model_definitions", "analytics"); 
context.ConfigureTenantEntity(model);

        model.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_predictive_model_definitions_tenant_id_id");

        model.Property(x => x.ModelKey).HasMaxLength(120); 
model.Property(x => x.Name).HasMaxLength(200);

        model.Property(x => x.TargetMetricCode).HasMaxLength(120); 
model.Property(x => x.Algorithm).HasMaxLength(120);

        model.Property(x => x.FeatureSchemaJson).HasColumnType("jsonb"); 
model.Property(x => x.ValidationMae).HasPrecision(28, 8);

        model.Property(x => x.ValidationMapePercent).HasPrecision(10, 4); 
model.Property(x => x.ValidationDatasetReference).HasMaxLength(500);

        model.Property(x => x.CreatedBy).HasMaxLength(240); 
model.Property(x => x.ApprovedBy).HasMaxLength(240);

        model.Property(x => x.ApprovalReference).HasMaxLength(240); 
model.Property(x => x.RetiredBy).HasMaxLength(240);

        model.HasIndex(x => new { x.TenantId, x.ModelKey, x.ModelVersion }).IsUnique();

        model.HasIndex(x => new { x.TenantId, x.TargetMetricCode, x.Status });

        modelBuilder.Entity<EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact>().HasOne<PredictiveModelDefinition>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PredictiveModelDefinitionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
