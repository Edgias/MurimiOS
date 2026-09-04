using EdgIQ.MurimiOS.Domain.RecallCase;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.RecallCase;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
internal static class RecallCaseConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var recall = modelBuilder.Entity<RecallCase>();

        recall.ToTable("recalls", "traceability");

        context.ConfigureTenantEntity(recall);

        recall.Property(x => x.Reference).HasMaxLength(120);

        recall.Property(x => x.Reason).HasMaxLength(2_000);

        recall.HasIndex(x => new { x.TenantId, x.Reference }).IsUnique();

        recall.HasIndex(x => new { x.TenantId, x.Status, x.InitiatedAt });

    }
}
