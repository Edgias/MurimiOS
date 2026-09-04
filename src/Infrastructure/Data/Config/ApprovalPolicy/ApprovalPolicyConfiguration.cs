using EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ApprovalPolicy;
using ApprovalPolicy = global::EdgIQ.MurimiOS.Domain.ApprovalPolicy.ApprovalPolicy;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class ApprovalPolicyConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var approvalPolicy = modelBuilder.Entity<ApprovalPolicy>();

        approvalPolicy.ToTable("approval_policies", "enterprise"); 
context.ConfigureTenantEntity(approvalPolicy);

        approvalPolicy.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_approval_policies_tenant_id_id");

        approvalPolicy.Property(x => x.Code).HasMaxLength(80); 
approvalPolicy.Property(x => x.Name).HasMaxLength(200);

        approvalPolicy.Property(x => x.ActionType).HasMaxLength(120); 
approvalPolicy.Property(x => x.ApproverPermission).HasMaxLength(120);

        approvalPolicy.Property(x => x.AuthoredBy).HasMaxLength(240); 
approvalPolicy.HasIndex(x => new { x.TenantId, x.Code, x.Status });

        approvalPolicy.HasOne<OrganisationNode>().WithMany().HasForeignKey(x => new { x.TenantId, x.OrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
