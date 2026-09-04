using EdgIQ.MurimiOS.Domain.ComplianceStandardVersion;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierEvidenceRequest;
using ComplianceControlPoint = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceControlPoint;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
internal static class SupplierEvidenceRequestConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var request = modelBuilder.Entity<SupplierEvidenceRequest>();

        request.ToTable("supplier_evidence_requests", "supply");

        context.ConfigureTenantEntity(request);

        request.Property(x => x.Title).HasMaxLength(240);

        request.Property(x => x.Description).HasMaxLength(4_000);

        request.Property(x => x.SupplierReference).HasMaxLength(500);

        request.Property(x => x.ReviewNotes).HasMaxLength(2_000);

        request.HasIndex(x => new { x.TenantId, x.Status, x.DueOn });

        request.HasIndex(x => new { x.TenantId, x.HostedSupplierId, x.Status });

        request.HasOne<SupplyRelationship>().WithMany().HasForeignKey(x => x.RelationshipId).OnDelete(DeleteBehavior.Restrict);

        request.HasOne<HostedSupplierAccount>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HostedSupplierId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        request.HasOne<SupplyContract>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ContractId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        request.HasOne<EvidenceAttachment>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.EvidenceAttachmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        request.HasOne<ComplianceStandardVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.StandardVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        request.HasOne<ComplianceControlPoint>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ControlPointId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
