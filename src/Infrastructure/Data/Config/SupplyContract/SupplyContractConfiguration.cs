using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
internal static class SupplyContractConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var contract = modelBuilder.Entity<SupplyContract>();

        contract.ToTable("contracts", "supply");

        context.ConfigureTenantEntity(contract);

        contract.Property(x => x.ContractNumber).HasMaxLength(80);

        contract.Property(x => x.Commodity).HasMaxLength(120);

        contract.Property(x => x.ContractedQuantity).HasPrecision(18, 4);

        contract.Property(x => x.QuantityUnit).HasMaxLength(30);

        contract.Property(x => x.UnitPrice).HasPrecision(18, 6);

        contract.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        contract.Property(x => x.TermsJson).HasColumnType("jsonb");

        contract.Property(x => x.TerminationReason).HasMaxLength(2_000);

        contract.HasIndex(x => new { x.TenantId, x.ContractNumber }).IsUnique();

        contract.HasIndex(x => new { x.TenantId, x.Status, x.ValidFrom, x.ValidTo });

        contract.HasOne<SupplyRelationship>().WithMany().HasForeignKey(x => x.RelationshipId).OnDelete(DeleteBehavior.Restrict);

        contract.HasOne<HostedSupplierAccount>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HostedSupplierId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
