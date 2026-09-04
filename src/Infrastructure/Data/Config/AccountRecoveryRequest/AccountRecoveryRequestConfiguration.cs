using EdgIQ.MurimiOS.Domain.AccountRecoveryRequest;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountRecoveryRequest;
using AccountRecoveryRequest = global::EdgIQ.MurimiOS.Domain.AccountRecoveryRequest.AccountRecoveryRequest;
internal static class AccountRecoveryRequestConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var recovery = modelBuilder.Entity<AccountRecoveryRequest>();

        recovery.ToTable("recovery_requests", "security"); 
context.ConfigureTenantEntity(recovery);

        recovery.Property(x => x.TokenPrefix).HasMaxLength(24);

        recovery.Property(x => x.TokenHash).HasMaxLength(64).IsFixedLength();

        recovery.Property(x => x.Subject).HasMaxLength(240);

        recovery.Property(x => x.RequestedBy).HasMaxLength(240);

        recovery.Property(x => x.ClosedBy).HasMaxLength(240);

        recovery.HasIndex(x => new { x.TenantId, x.TokenHash }).IsUnique();

        recovery.HasIndex(x => new { x.TenantId, x.MembershipId, x.Status });

    }
}
