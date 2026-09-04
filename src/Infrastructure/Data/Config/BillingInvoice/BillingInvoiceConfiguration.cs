using EdgIQ.MurimiOS.Domain.TenantSubscription;
using EdgIQ.MurimiOS.Domain.BillingAccount;
using EdgIQ.MurimiOS.Domain.BillingInvoice;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.BillingInvoice;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;
using Tax = global::EdgIQ.MurimiOS.Domain.Tax.Tax;

using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;
using BillingInvoice = global::EdgIQ.MurimiOS.Domain.BillingInvoice.BillingInvoice;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
internal static class BillingInvoiceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var invoice = modelBuilder.Entity<BillingInvoice>();

        invoice.ToTable("invoices", "saas"); 
context.ConfigureTenantEntity(invoice);

        invoice.Property(x => x.InvoiceNumber).HasMaxLength(120);

        invoice.Property(x => x.ProviderInvoiceReference).HasMaxLength(200);

        invoice.Property(x => x.Subtotal).HasPrecision(18, 2);

        invoice.Property(x => x.Tax).HasPrecision(18, 2);

        invoice.Property(x => x.Total).HasPrecision(18, 2);

        invoice.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        invoice.Property(x => x.StatusReason).HasMaxLength(1_000);

        invoice.HasIndex(x => new { x.TenantId, x.InvoiceNumber }).IsUnique();

        invoice.HasIndex(x => new { x.TenantId, x.ProviderInvoiceReference }).IsUnique()
            .HasFilter("provider_invoice_reference IS NOT NULL");

        invoice.HasIndex(x => new { x.TenantId, x.Status, x.DueAt });

        invoice.HasOne<BillingAccount>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BillingAccountId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        invoice.HasOne<TenantSubscription>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubscriptionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
