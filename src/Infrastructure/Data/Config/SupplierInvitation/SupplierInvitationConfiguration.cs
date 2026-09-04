using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierInvitation;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;
internal static class SupplierInvitationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var invitation = modelBuilder.Entity<SupplierInvitation>();

        invitation.ToTable("supplier_invitations", "supply");

        context.ConfigureTenantEntity(invitation);

        invitation.Property(x => x.SupplierName).HasMaxLength(200);

        invitation.Property(x => x.Email).HasMaxLength(320);

        invitation.Property(x => x.TokenHash).HasMaxLength(64).IsFixedLength();

        invitation.Property(x => x.InvitedBy).HasMaxLength(240);

        invitation.HasIndex(x => new { x.TenantId, x.TokenHash }).IsUnique();

        invitation.HasIndex(x => new { x.TenantId, x.Email, x.Status });

    }
}
