using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplierSelfServiceToken;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
internal static class SupplierSelfServiceTokenConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var selfService = modelBuilder.Entity<SupplierSelfServiceToken>();

        selfService.ToTable("supplier_self_service_tokens", "supply");

        context.ConfigureTenantEntity(selfService);

        selfService.Property(x => x.TokenHash).HasMaxLength(64).IsFixedLength();

        selfService.Property(x => x.IssuedBy).HasMaxLength(240);

        selfService.Property(x => x.ConsumerNetworkHash).HasMaxLength(64).IsFixedLength();

        selfService.HasIndex(x => new { x.TenantId, x.TokenHash }).IsUnique();

        selfService.HasIndex(x => new { x.TenantId, x.HostedSupplierId, x.Status, x.ExpiresAt });

        selfService.HasOne<HostedSupplierAccount>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HostedSupplierId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
