using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject;
using LivestockIdentifier = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockIdentifier;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockIdentifierConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var identifier = modelBuilder.Entity<LivestockIdentifier>();

        identifier.ToTable("identifiers", "livestock");

        context.ConfigureTenantEntity(identifier);

        identifier.Property(x => x.IdentifierNamespace).HasMaxLength(120);

        identifier.Property(x => x.Value).HasMaxLength(160);

        identifier.HasIndex(x => new { x.TenantId, x.IdentifierNamespace, x.Value }).IsUnique();

        identifier.HasIndex(x => new { x.TenantId, x.SubjectId, x.IsPrimary, x.ValidTo });

        identifier.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
