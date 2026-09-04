using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class HostedSupplierAccount : TenantEntity
{
    private HostedSupplierAccount() { }
    public HostedSupplierAccount(Guid id, string supplierCode, string name, string contactName, string email, string? phone) : base(id)
    {
        SupplierCode = SupplierInvitation.Text(supplierCode, 60); Name = SupplierInvitation.Text(name, 200);
        ContactName = SupplierInvitation.Text(contactName, 200); Email = SupplierInvitation.Text(email, 320).ToLowerInvariant();
        Phone = SupplierInvitation.Optional(phone, 60); Status = HostedSupplierStatus.Active;
    }
    public string SupplierCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string ContactName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public HostedSupplierStatus Status { get; private set; }
    public Guid? ClaimedTenantId { get; private set; }
    public DateTimeOffset? ClaimedAt { get; private set; }
    public DateTimeOffset? LastDetachedAt { get; private set; }
    public void Suspend() { if (Status != HostedSupplierStatus.Active) throw new InvalidOperationException("Only an active hosted supplier can be suspended."); Status = HostedSupplierStatus.Suspended; }
    public void Reactivate() { if (Status != HostedSupplierStatus.Suspended) throw new InvalidOperationException("Only a suspended hosted supplier can be reactivated."); Status = HostedSupplierStatus.Active; }
    public void Claim(Guid tenantId, DateTimeOffset at) { if (Status != HostedSupplierStatus.Active || tenantId == Guid.Empty) throw new InvalidOperationException("Only an active hosted supplier can be claimed."); Status = HostedSupplierStatus.Claimed; ClaimedTenantId = tenantId; ClaimedAt = at.ToUniversalTime(); }
    public void Detach(Guid tenantId, DateTimeOffset at) { if (Status != HostedSupplierStatus.Claimed || ClaimedTenantId != tenantId) throw new InvalidOperationException("Only the linked supplier tenant can detach this account."); Status = HostedSupplierStatus.Active; ClaimedTenantId = null; ClaimedAt = null; LastDetachedAt = at.ToUniversalTime(); }
}
