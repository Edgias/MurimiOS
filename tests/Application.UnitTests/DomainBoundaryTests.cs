using EdgIQ.MurimiOS.Application;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Application.UnitTests;

public sealed class DomainBoundaryTests
{
    [Fact]
    public void Application_facing_contracts_belong_to_application_not_domain()
    {
        Type[] contracts =
        [
            typeof(ITenantContext), typeof(IAuditActorContext), typeof(IAppLogger<>),
            typeof(IFileSystem), typeof(IMessagingService), typeof(IQuotationService),
            typeof(ISalesOrderService), typeof(ISalesInvoiceService)
        ];

        Assert.All(contracts, contract =>
        {
            Assert.True(contract.IsInterface);
            Assert.Equal(typeof(ApplicationAssemblyMarker).Assembly, contract.Assembly);
            Assert.Equal("EdgIQ.MurimiOS.Application.Common.Abstractions", contract.Namespace);
        });
    }

    [Fact]
    public void Domain_does_not_reference_application_infrastructure_http_or_ef()
    {
        string[] references = typeof(BaseEntity).Assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty).ToArray();

        Assert.DoesNotContain("Application", references);
        Assert.DoesNotContain("Infrastructure", references);
        Assert.DoesNotContain("API", references);
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal));
    }

    [Fact]
    public void Domain_exports_use_the_approved_namespace_prefix()
    {
        // A folder called Bin is excluded by SDK globs on Windows; keep this
        // entity compiled from the Bins aggregate directory.
        Assert.Equal("Bin", typeof(global::EdgIQ.MurimiOS.Domain.Bins.Bin).Name);
        Assert.All(typeof(BaseEntity).Assembly.GetExportedTypes(), type =>
            Assert.StartsWith("EdgIQ.MurimiOS.Domain.", type.Namespace));
    }

    [Fact]
    public void Legacy_event_and_distinct_aggregate_contracts_are_preserved()
    {
        Assert.True(typeof(MediatR.INotification).IsAssignableFrom(typeof(BaseDomainEvent)));
        Type legacyTreatment = typeof(global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockTreatment);
        Type activeTreatment = typeof(global::EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment);
        Assert.Equal(legacyTreatment.Name, activeTreatment.Name);
        Assert.NotEqual(legacyTreatment.FullName, activeTreatment.FullName);

        Type purchaseItem = typeof(global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.InvoicedItem);
        Type salesItem = typeof(global::EdgIQ.MurimiOS.Domain.SalesInvoice.InvoicedItem);
        Assert.Equal(purchaseItem.Name, salesItem.Name);
        Assert.NotEqual(purchaseItem.FullName, salesItem.FullName);
    }
}
