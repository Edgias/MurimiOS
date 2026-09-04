using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockReproductionEvent;
using LivestockOffspringLink = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockOffspringLink;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockOffspringLinkConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var offspring = modelBuilder.Entity<LivestockOffspringLink>();

        offspring.ToTable("offspring_links", "livestock");

        context.ConfigureTenantEntity(offspring);

        offspring.Property(x => x.BirthWeightKg).HasPrecision(18, 4);

        offspring.Property(x => x.EnteredBirthWeight).HasPrecision(18, 4);

        offspring.Property(x => x.EnteredBirthWeightUnit).HasMaxLength(30);

        offspring.HasIndex(x => new { x.TenantId, x.ParturitionEventId, x.OffspringSubjectId }).IsUnique();

        offspring.HasIndex(x => new { x.TenantId, x.OffspringSubjectId }).IsUnique();

        offspring.HasOne<LivestockReproductionEvent>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ParturitionEventId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        offspring.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.OffspringSubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
