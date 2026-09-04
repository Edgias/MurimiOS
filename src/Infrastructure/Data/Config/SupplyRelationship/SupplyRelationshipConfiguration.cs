using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyRelationship;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
internal static class SupplyRelationshipConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var relationship = modelBuilder.Entity<SupplyRelationship>();

        relationship.ToTable("relationships", "supply");

        relationship.HasKey(x => x.Id);

        relationship.Property(x => x.Id).ValueGeneratedNever();

        relationship.Property(x => x.RelationshipName).HasMaxLength(200);

        relationship.Property(x => x.RevocationReason).HasMaxLength(1_000);

        relationship.Property(x => x.Version).IsConcurrencyToken();

        relationship.HasIndex(x => new { x.ProducerTenantId, x.BuyerTenantId, x.Status });

        relationship.HasIndex(x => new { x.BuyerTenantId, x.Status, x.RequestedAt });

    }
}
