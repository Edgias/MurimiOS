using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class EudrDueDiligenceCaseConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var dueDiligenceCase = modelBuilder.Entity<EudrDueDiligenceCase>();

        dueDiligenceCase.ToTable("due_diligence_cases", "eudr");

        context.ConfigureTenantEntity(dueDiligenceCase);

        dueDiligenceCase.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_eudr_cases_tenant_id_id");

        dueDiligenceCase.Property(x => x.CaseNumber).HasMaxLength(80);

        dueDiligenceCase.Property(x => x.ProductDescription).HasMaxLength(500);

        dueDiligenceCase.Property(x => x.HsCode).HasMaxLength(20);

        dueDiligenceCase.Property(x => x.Quantity).HasPrecision(18, 4);

        dueDiligenceCase.Property(x => x.Unit).HasMaxLength(30);

        dueDiligenceCase.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        dueDiligenceCase.Property(x => x.EnteredUnit).HasMaxLength(30);

        dueDiligenceCase.Property(x => x.ProductionCountry).HasMaxLength(2).IsFixedLength();

        dueDiligenceCase.Property(x => x.OperatorName).HasMaxLength(240);

        dueDiligenceCase.Property(x => x.OperatorRegistrationNumber).HasMaxLength(120);

        dueDiligenceCase.Property(x => x.OperatorAddress).HasMaxLength(500);

        dueDiligenceCase.Property(x => x.Market).HasMaxLength(120);

        dueDiligenceCase.Property(x => x.CreatedBy).HasMaxLength(240);

        dueDiligenceCase.Property(x => x.SubmissionReference).HasMaxLength(200);

        dueDiligenceCase.Property(x => x.CancellationReason).HasMaxLength(2_000);

        dueDiligenceCase.HasIndex(x => new { x.TenantId, x.CaseNumber }).IsUnique();

        dueDiligenceCase.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.CreatedAt });

        dueDiligenceCase.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
