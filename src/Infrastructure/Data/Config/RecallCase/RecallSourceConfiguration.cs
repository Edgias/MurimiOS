using EdgIQ.MurimiOS.Domain.RecallCase;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.RecallCase;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallSource = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallSource;
internal static class RecallSourceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var recallSource = modelBuilder.Entity<RecallSource>();

        recallSource.ToTable("recall_sources", "traceability");

        context.ConfigureTenantEntity(recallSource);

        recallSource.HasIndex(x => new { x.TenantId, x.RecallCaseId, x.ObjectType, x.ObjectId }).IsUnique();

        recallSource.HasOne<RecallCase>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.RecallCaseId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
