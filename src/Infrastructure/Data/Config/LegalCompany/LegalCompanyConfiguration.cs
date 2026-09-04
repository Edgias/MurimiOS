using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LegalCompany;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class LegalCompanyConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var company = modelBuilder.Entity<LegalCompany>();

        company.ToTable("legal_companies", "finance");

        context.ConfigureTenantEntity(company);

        company.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_finance_legal_companies_tenant_id_id");

        company.Property(x => x.CompanyCode).HasMaxLength(80);

        company.Property(x => x.LegalName).HasMaxLength(240);

        company.Property(x => x.RegistrationNumber).HasMaxLength(120);

        company.Property(x => x.TaxIdentifier).HasMaxLength(120);

        company.Property(x => x.CountryCode).HasMaxLength(2).IsFixedLength();

        company.Property(x => x.FunctionalCurrency).HasMaxLength(3).IsFixedLength();

        company.Property(x => x.CreatedBy).HasMaxLength(240);

        company.Property(x => x.ClosedBy).HasMaxLength(240);

        company.HasIndex(x => new { x.TenantId, x.OrganisationNodeId }).IsUnique();

        company.HasIndex(x => new { x.TenantId, x.CompanyCode }).IsUnique();

        company.HasIndex(x => new { x.TenantId, x.CountryCode, x.RegistrationNumber }).IsUnique();

        company.HasOne<OrganisationNode>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.OrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
