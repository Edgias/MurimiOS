using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.HostedSupplierAccount;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using HostedSupplierLinkEvent = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierLinkEvent;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
internal static class HostedSupplierLinkEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var linkEvent = modelBuilder.Entity<HostedSupplierLinkEvent>();

        linkEvent.ToTable("hosted_supplier_link_events", "supply");

        context.ConfigureTenantEntity(linkEvent);

        linkEvent.HasIndex(x => new { x.TenantId, x.HostedSupplierId, x.OccurredAt });

        linkEvent.HasIndex(x => new { x.TenantId, x.SelfServiceTokenId }).IsUnique();

        linkEvent.HasOne<HostedSupplierAccount>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HostedSupplierId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        linkEvent.HasOne<SupplierSelfServiceToken>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SelfServiceTokenId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
