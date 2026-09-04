using EdgIQ.MurimiOS.Domain.IdentityDevice;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IdentitySession;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
internal static class IdentitySessionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var session = modelBuilder.Entity<IdentitySession>();

        session.ToTable("sessions", "security"); 
context.ConfigureTenantEntity(session);

        session.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_security_sessions_tenant_id_id");

        session.Property(x => x.SessionKeyHash).HasMaxLength(64).IsFixedLength();

        session.Property(x => x.Subject).HasMaxLength(240);

        session.Property(x => x.AuthenticationMethod).HasMaxLength(80);

        session.Property(x => x.RemoteAddressHash).HasMaxLength(64).IsFixedLength();

        session.Property(x => x.UserAgentHash).HasMaxLength(64).IsFixedLength();

        session.Property(x => x.RevokedBy).HasMaxLength(240);

        session.Property(x => x.RevocationReason).HasMaxLength(1_000);

        session.HasIndex(x => new { x.TenantId, x.SessionKeyHash }).IsUnique();

        session.HasIndex(x => new { x.TenantId, x.Subject, x.Status, x.ExpiresAt });

        session.HasOne<IdentityDevice>().WithMany().HasForeignKey(x => new { x.TenantId, x.DeviceId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
