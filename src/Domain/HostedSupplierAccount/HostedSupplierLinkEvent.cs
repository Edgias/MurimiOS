using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using SupplierSelfServiceAction = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceAction;

public sealed class HostedSupplierLinkEvent : TenantEntity
{
    private HostedSupplierLinkEvent() { }

    public HostedSupplierLinkEvent(Guid id, Guid hostedSupplierId, Guid selfServiceTokenId,
        SupplierSelfServiceAction action, Guid claimedTenantId, DateTimeOffset occurredAt) : base(id)
    {
        if (hostedSupplierId == Guid.Empty || selfServiceTokenId == Guid.Empty || claimedTenantId == Guid.Empty
            || !Enum.IsDefined(action)) throw new ArgumentException("A supplier link event requires valid identifiers and action.");
        HostedSupplierId = hostedSupplierId;
        SelfServiceTokenId = selfServiceTokenId;
        Action = action;
        ClaimedTenantId = claimedTenantId;
        OccurredAt = occurredAt.ToUniversalTime();
    }

    public Guid HostedSupplierId { get; private set; }
    public Guid SelfServiceTokenId { get; private set; }
    public SupplierSelfServiceAction Action { get; private set; }
    public Guid ClaimedTenantId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
}
