using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.BillingAccount;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class BillingAccount : TenantEntity
{
    private BillingAccount() { }

    public BillingAccount(Guid id, string provider, string providerCustomerReference, string billingEmail,
        string? taxIdentifier, DateTimeOffset createdAt) : base(id)
    {
        Provider = TenantAccount.Code(provider, 80);
        ProviderCustomerReference = TenantAccount.Text(providerCustomerReference, 200);
        BillingEmail = TenantAccount.Text(billingEmail, 320).ToLowerInvariant();
        TaxIdentifier = NullIfEmpty(taxIdentifier, 120);
        CreatedAt = createdAt.ToUniversalTime();
        Status = BillingAccountStatus.Active;
    }

    public string Provider { get; private set; } = "";
    public string ProviderCustomerReference { get; private set; } = "";
    public string BillingEmail { get; private set; } = "";
    public string? TaxIdentifier { get; private set; }
    public BillingAccountStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public void Suspend() { if (Status != BillingAccountStatus.Active) throw new InvalidOperationException("Billing account is not active."); Status = BillingAccountStatus.Suspended; }
    public void Activate() { if (Status != BillingAccountStatus.Suspended) throw new InvalidOperationException("Billing account is not suspended."); Status = BillingAccountStatus.Active; }
    public void Close() { if (Status == BillingAccountStatus.Closed) throw new InvalidOperationException("Billing account is already closed."); Status = BillingAccountStatus.Closed; }

    private static string? NullIfEmpty(string? value, int maximum)
    { string text = TenantAccount.Optional(value, maximum); return text.Length == 0 ? null : text; }
}
