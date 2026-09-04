using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.HostedSupplierAccount;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
internal static class HostedSupplierAccountConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var hosted = modelBuilder.Entity<HostedSupplierAccount>();

        hosted.ToTable("hosted_suppliers", "supply");

        context.ConfigureTenantEntity(hosted);

        hosted.Property(x => x.SupplierCode).HasMaxLength(60);

        hosted.Property(x => x.Name).HasMaxLength(200);

        hosted.Property(x => x.ContactName).HasMaxLength(200);

        hosted.Property(x => x.Email).HasMaxLength(320);

        hosted.Property(x => x.Phone).HasMaxLength(60);

        hosted.HasIndex(x => new { x.TenantId, x.SupplierCode }).IsUnique();

        hosted.HasIndex(x => new { x.TenantId, x.Email });


        modelBuilder.Entity<EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation>().HasOne<HostedSupplierAccount>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HostedSupplierId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
