using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class SupplierEvidenceRequest : TenantEntity
{
    private SupplierEvidenceRequest() { }
    public SupplierEvidenceRequest(Guid id, Guid? relationshipId, Guid? hostedSupplierId, Guid? contractId,
        Guid? standardVersionId, Guid? controlPointId, string title, string description, DateOnly dueOn) : base(id)
    {
        if (relationshipId.HasValue == hostedSupplierId.HasValue || relationshipId == Guid.Empty || hostedSupplierId == Guid.Empty || contractId == Guid.Empty || standardVersionId == Guid.Empty || controlPointId == Guid.Empty) throw new ArgumentException("A valid supplier target and references are required.");
        RelationshipId = relationshipId; HostedSupplierId = hostedSupplierId; ContractId = contractId;
        StandardVersionId = standardVersionId; ControlPointId = controlPointId; Title = SupplierInvitation.Text(title, 240);
        Description = SupplierInvitation.Text(description, 4_000); DueOn = dueOn; Status = SupplierEvidenceRequestStatus.Requested;
    }
    public Guid? RelationshipId { get; private set; }
    public Guid? HostedSupplierId { get; private set; }
    public Guid? ContractId { get; private set; }
    public Guid? StandardVersionId { get; private set; }
    public Guid? ControlPointId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateOnly DueOn { get; private set; }
    public SupplierEvidenceRequestStatus Status { get; private set; }
    public Guid? EvidenceAttachmentId { get; private set; }
    public string? SupplierReference { get; private set; }
    public string? ReviewNotes { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public void Submit(Guid? evidenceAttachmentId, string? supplierReference, DateTimeOffset at) { if (Status is not (SupplierEvidenceRequestStatus.Requested or SupplierEvidenceRequestStatus.Rejected) || (evidenceAttachmentId is null || evidenceAttachmentId == Guid.Empty) && string.IsNullOrWhiteSpace(supplierReference)) throw new InvalidOperationException("The evidence request cannot be submitted."); EvidenceAttachmentId = evidenceAttachmentId; SupplierReference = SupplierInvitation.Optional(supplierReference, 500); Status = SupplierEvidenceRequestStatus.Submitted; SubmittedAt = at.ToUniversalTime(); ReviewNotes = null; }
    public void Accept(DateTimeOffset at, string? notes) { if (Status != SupplierEvidenceRequestStatus.Submitted) throw new InvalidOperationException("Only submitted evidence can be accepted."); Status = SupplierEvidenceRequestStatus.Accepted; ReviewedAt = at.ToUniversalTime(); ReviewNotes = SupplierInvitation.Optional(notes, 2_000); }
    public void Reject(DateTimeOffset at, string notes) { if (Status != SupplierEvidenceRequestStatus.Submitted) throw new InvalidOperationException("Only submitted evidence can be rejected."); Status = SupplierEvidenceRequestStatus.Rejected; ReviewedAt = at.ToUniversalTime(); ReviewNotes = SupplierInvitation.Text(notes, 2_000); }
    public void Cancel() { if (Status == SupplierEvidenceRequestStatus.Accepted) throw new InvalidOperationException("Accepted evidence cannot be cancelled."); Status = SupplierEvidenceRequestStatus.Cancelled; }
}
