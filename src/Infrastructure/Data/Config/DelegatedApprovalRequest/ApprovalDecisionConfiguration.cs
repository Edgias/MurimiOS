using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.DelegatedApprovalRequest;
using ApprovalDecision = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecision;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
internal static class ApprovalDecisionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var decision = modelBuilder.Entity<ApprovalDecision>();

        decision.ToTable("approval_decisions", "enterprise"); 
context.ConfigureTenantEntity(decision);

        decision.Property(x => x.ActorSubject).HasMaxLength(240); 
decision.Property(x => x.Reason).HasMaxLength(2000);

        decision.HasIndex(x => new { x.TenantId, x.RequestId, x.MembershipId }).IsUnique();

        decision.HasOne<DelegatedApprovalRequest>().WithMany().HasForeignKey(x => new { x.TenantId, x.RequestId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        decision.HasOne<TenantMembership>().WithMany().HasForeignKey(x => new { x.TenantId, x.MembershipId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
