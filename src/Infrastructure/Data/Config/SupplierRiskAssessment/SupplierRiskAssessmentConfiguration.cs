using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierRiskAssessment;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplierRiskAssessment = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskAssessment;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
internal static class SupplierRiskAssessmentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var risk = modelBuilder.Entity<SupplierRiskAssessment>();

        risk.ToTable("supplier_risk_assessments", "supply");

        context.ConfigureTenantEntity(risk);

        risk.Property(x => x.ModelVersion).HasMaxLength(40);

        risk.Property(x => x.ComponentsJson).HasColumnType("jsonb");

        risk.Property(x => x.InputsJson).HasColumnType("jsonb");

        risk.Property(x => x.AssessedBy).HasMaxLength(240);

        risk.HasIndex(x => new { x.TenantId, x.HostedSupplierId, x.AssessedAt });

        risk.HasIndex(x => new { x.TenantId, x.RelationshipId, x.AssessedAt });

        risk.HasIndex(x => new { x.TenantId, x.Band, x.AssessedAt });

        risk.HasOne<SupplyRelationship>().WithMany().HasForeignKey(x => x.RelationshipId).OnDelete(DeleteBehavior.Restrict);

        risk.HasOne<HostedSupplierAccount>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HostedSupplierId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
