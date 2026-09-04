using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockDispatch;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockDispatchConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var dispatch = modelBuilder.Entity<LivestockDispatch>();

        dispatch.ToTable("dispatches", "livestock");

        context.ConfigureTenantEntity(dispatch);

        dispatch.Property(x => x.Destination).HasMaxLength(240);

        dispatch.Property(x => x.OverrideReason).HasMaxLength(1_000);

        dispatch.Property(x => x.OverrideAuthorizedBy).HasMaxLength(200);

        dispatch.HasIndex(x => new { x.TenantId, x.SubjectId, x.DispatchedAt });

        dispatch.HasOne<LivestockSubject>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
