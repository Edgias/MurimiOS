using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.BillingInvoice;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class BillingInvoice : TenantEntity
{
    private BillingInvoice() { }

    public BillingInvoice(Guid id, Guid billingAccountId, Guid? subscriptionId, string invoiceNumber,
        string? providerInvoiceReference, decimal subtotal, decimal tax, decimal total, string currency,
        DateTimeOffset issuedAt, DateTimeOffset dueAt) : base(id)
    {
        if (billingAccountId == Guid.Empty || subscriptionId == Guid.Empty || subtotal < 0 || tax < 0 || total != subtotal + tax || dueAt < issuedAt)
            throw new ArgumentException("Invoice account, totals, or dates are invalid.");
        BillingAccountId = billingAccountId;
        SubscriptionId = subscriptionId;
        InvoiceNumber = TenantAccount.Text(invoiceNumber, 120);
        ProviderInvoiceReference = NullIfEmpty(providerInvoiceReference, 200);
        Subtotal = subtotal;
        Tax = tax;
        Total = total;
        Currency = TenantAccount.Currency(currency);
        IssuedAt = issuedAt.ToUniversalTime();
        DueAt = dueAt.ToUniversalTime();
        Status = BillingInvoiceStatus.Open;
    }

    public Guid BillingAccountId { get; private set; }
    public Guid? SubscriptionId { get; private set; }
    public string InvoiceNumber { get; private set; } = "";
    public string? ProviderInvoiceReference { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Total { get; private set; }
    public string Currency { get; private set; } = "";
    public BillingInvoiceStatus Status { get; private set; }
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset DueAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? StatusReason { get; private set; }

    public void MarkPaid(DateTimeOffset at) { if (Status != BillingInvoiceStatus.Open) throw new InvalidOperationException("Only an open invoice can be paid."); Status = BillingInvoiceStatus.Paid; PaidAt = at.ToUniversalTime(); ClosedAt = PaidAt; }
    public void Void(DateTimeOffset at, string reason) { if (Status is BillingInvoiceStatus.Paid or BillingInvoiceStatus.Void) throw new InvalidOperationException("Paid or void invoices cannot be voided."); Status = BillingInvoiceStatus.Void; ClosedAt = at.ToUniversalTime(); StatusReason = TenantAccount.Text(reason, 1_000); }
    public void MarkUncollectible(DateTimeOffset at, string reason) { if (Status != BillingInvoiceStatus.Open) throw new InvalidOperationException("Only an open invoice can become uncollectible."); Status = BillingInvoiceStatus.Uncollectible; ClosedAt = at.ToUniversalTime(); StatusReason = TenantAccount.Text(reason, 1_000); }

    private static string? NullIfEmpty(string? value, int maximum)
    { string text = TenantAccount.Optional(value, maximum); return text.Length == 0 ? null : text; }
}
