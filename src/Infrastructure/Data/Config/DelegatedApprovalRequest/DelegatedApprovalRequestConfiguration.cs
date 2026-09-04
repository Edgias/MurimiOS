using EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.DelegatedApprovalRequest;
using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class DelegatedApprovalRequestConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var approvalRequest = modelBuilder.Entity<DelegatedApprovalRequest>();

        approvalRequest.ToTable("approval_requests", "enterprise"); 
context.ConfigureTenantEntity(approvalRequest);

        approvalRequest.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_approval_requests_tenant_id_id");

        approvalRequest.Property(x => x.TargetType).HasMaxLength(120); 
approvalRequest.Property(x => x.ActionType).HasMaxLength(120);

        approvalRequest.Property(x => x.RequestJson).HasColumnType("jsonb"); 
approvalRequest.Property(x => x.RequestedBy).HasMaxLength(240);

        approvalRequest.HasIndex(x => new { x.TenantId, x.Status, x.ExpiresAt });

        approvalRequest.HasIndex(x => new { x.TenantId, x.TargetType, x.TargetId, x.ActionType, x.Status });

        approvalRequest.HasOne<ApprovalPolicy>().WithMany().HasForeignKey(x => new { x.TenantId, x.PolicyId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        approvalRequest.HasOne<OrganisationNode>().WithMany().HasForeignKey(x => new { x.TenantId, x.OrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        approvalRequest.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
