using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.DataGrant;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
internal static class DataGrantConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var grant = modelBuilder.Entity<DataGrant>();

        grant.ToTable("data_grants", "supply");

        grant.HasKey(x => x.Id);

        grant.Property(x => x.Id).ValueGeneratedNever();

        grant.Property(x => x.Name).HasMaxLength(200);

        grant.Property(x => x.Purpose).HasMaxLength(240);

        grant.Property(x => x.EntityCategories).HasColumnType("text[]");

        grant.Property(x => x.Attributes).HasColumnType("text[]");

        grant.Property(x => x.FarmIds).HasColumnType("uuid[]");

        grant.Property(x => x.SeasonIds).HasColumnType("uuid[]");

        grant.Property(x => x.Commodities).HasColumnType("text[]");

        grant.Property(x => x.RevocationReason).HasMaxLength(1_000);

        grant.Property(x => x.Version).IsConcurrencyToken();

        grant.HasIndex(x => new { x.ProducerTenantId, x.RecipientTenantId, x.Status, x.ValidFrom, x.ValidTo });

        grant.HasIndex(x => new { x.GrantSeriesId, x.GrantVersion }).IsUnique();

        grant.HasOne<SupplyRelationship>().WithMany().HasForeignKey(x => x.RelationshipId).OnDelete(DeleteBehavior.Restrict);

        grant.HasOne<DataGrant>().WithMany().HasForeignKey(x => x.PreviousGrantId).OnDelete(DeleteBehavior.Restrict);

    }
}
