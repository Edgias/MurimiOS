using EdgIQ.MurimiOS.Domain.IdentityDevice;
using EdgIQ.MurimiOS.Domain.IdentitySession;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SecurityEvent;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
using IdentitySession = global::EdgIQ.MurimiOS.Domain.IdentitySession.IdentitySession;
using SecurityEvent = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEvent;
internal static class SecurityEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var securityEvent = modelBuilder.Entity<SecurityEvent>();

        securityEvent.ToTable("events", "security"); 
context.ConfigureTenantEntity(securityEvent);

        securityEvent.Property(x => x.EventType).HasMaxLength(120);

        securityEvent.Property(x => x.Subject).HasMaxLength(240);

        securityEvent.Property(x => x.CorrelationId).HasMaxLength(160);

        securityEvent.Property(x => x.RemoteAddressHash).HasMaxLength(64).IsFixedLength();

        securityEvent.Property(x => x.UserAgentHash).HasMaxLength(64).IsFixedLength();

        securityEvent.Property(x => x.DetailsJson).HasColumnType("jsonb");

        securityEvent.HasIndex(x => new { x.TenantId, x.OccurredAt });

        securityEvent.HasIndex(x => new { x.TenantId, x.EventType, x.Outcome, x.OccurredAt });

        securityEvent.HasOne<IdentitySession>().WithMany().HasForeignKey(x => new { x.TenantId, x.SessionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        securityEvent.HasOne<IdentityDevice>().WithMany().HasForeignKey(x => new { x.TenantId, x.DeviceId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
