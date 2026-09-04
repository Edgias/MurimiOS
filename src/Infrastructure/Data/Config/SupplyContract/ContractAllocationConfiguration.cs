using EdgIQ.MurimiOS.Domain.SupplyContract;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract;
using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
internal static class ContractAllocationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var allocation = modelBuilder.Entity<ContractAllocation>();

        allocation.ToTable("contract_allocations", "supply");

        context.ConfigureTenantEntity(allocation);

        allocation.Property(x => x.Reference).HasMaxLength(120);

        allocation.Property(x => x.AllocatedQuantity).HasPrecision(18, 4);

        allocation.Property(x => x.FulfilledQuantity).HasPrecision(18, 4);

        allocation.Property(x => x.QuantityUnit).HasMaxLength(30);

        allocation.HasIndex(x => new { x.TenantId, x.ContractId, x.Reference }).IsUnique();

        allocation.HasIndex(x => new { x.TenantId, x.Status, x.DeliveryFrom, x.DeliveryTo });

        allocation.HasOne<SupplyContract>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ContractId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
