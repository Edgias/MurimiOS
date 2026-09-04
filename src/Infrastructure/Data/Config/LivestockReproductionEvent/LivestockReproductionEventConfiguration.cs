using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockReproductionEvent;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockReproductionEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var reproduction = modelBuilder.Entity<LivestockReproductionEvent>();

        reproduction.ToTable("reproduction_events", "livestock");

        context.ConfigureTenantEntity(reproduction);

        reproduction.Property(x => x.Method).HasMaxLength(120);

        reproduction.Property(x => x.WeightKg).HasPrecision(18, 4);

        reproduction.Property(x => x.EnteredWeight).HasPrecision(18, 4);

        reproduction.Property(x => x.EnteredWeightUnit).HasMaxLength(30);

        reproduction.Property(x => x.Practitioner).HasMaxLength(200);

        reproduction.Property(x => x.Notes).HasMaxLength(2_000);

        reproduction.HasIndex(x => new { x.TenantId, x.SubjectId, x.OccurredAt });

        reproduction.HasIndex(x => new { x.TenantId, x.FarmId, x.Type, x.OccurredAt });

        reproduction.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reproduction.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reproduction.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.RelatedSubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reproduction.HasOne<LivestockReproductionEvent>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BreedingEventId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
