using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateEvidenceRequest;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;

using HostedSupplierStatus = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierStatus;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class CreateEvidenceRequestHandler(IPlatformDbContext db, ITenantContext tenant) : IRequestHandler<CreateEvidenceRequestCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateEvidenceRequestCommand message, CancellationToken cancellationToken) => CreateEvidenceRequestAsync(message.request, db, tenant, cancellationToken);

    private static async Task<UseCaseResult> CreateEvidenceRequestAsync(CreateSupplierEvidenceRequestRequest request,
        IPlatformDbContext db, ITenantContext tenant, CancellationToken ct)
    {
        UseCaseResult? targetError = await ValidateTargetAsync(request.RelationshipId, request.HostedSupplierId, db, tenant, ct);
        if (targetError is not null) return targetError;
        if (request.ContractId.HasValue && !await db.SupplyContracts.AnyAsync(x => x.Id == request.ContractId
            && x.RelationshipId == request.RelationshipId && x.HostedSupplierId == request.HostedSupplierId, ct))
            return Bad("contract_target_mismatch", "The contract does not belong to the selected supplier.");
        if (request.StandardVersionId.HasValue && !await db.ComplianceStandardVersions.AnyAsync(x => x.Id == request.StandardVersionId, ct))
            return Bad("standard_invalid", "The standard version is not available to this tenant.");
        if (request.ControlPointId.HasValue && !await db.ComplianceControlPoints.AnyAsync(x => x.Id == request.ControlPointId
            && (!request.StandardVersionId.HasValue || x.StandardVersionId == request.StandardVersionId), ct))
            return Bad("control_invalid", "The control point is not part of the selected standard.");
        try
        {
            var evidenceRequest = new SupplierEvidenceRequest(Id(request.Id), request.RelationshipId, request.HostedSupplierId,
                request.ContractId, request.StandardVersionId, request.ControlPointId, request.Title, request.Description, request.DueOn);
            db.SupplierEvidenceRequests.Add(evidenceRequest); await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/supplier-portal/evidence-requests/{evidenceRequest.Id}", evidenceRequest);
        }
        catch (ArgumentException e) { return Bad("evidence_request_invalid", e.Message); }
    }


    private static async Task<UseCaseResult?> ValidateTargetAsync(Guid? relationshipId, Guid? hostedSupplierId,
        IPlatformDbContext db, ITenantContext tenant, CancellationToken ct)
    {
        if (relationshipId.HasValue == hostedSupplierId.HasValue) return Bad("supplier_target_invalid", "Choose either an active tenant relationship or hosted supplier.");
        if (relationshipId.HasValue && !await db.SupplyRelationships.AnyAsync(x => x.Id == relationshipId && x.Status == SupplyRelationshipStatus.Active
            && (x.ProducerTenantId == tenant.TenantId || x.BuyerTenantId == tenant.TenantId), ct)) return Bad("relationship_invalid", "The active supply relationship is not available.");
        if (hostedSupplierId.HasValue && !await db.HostedSupplierAccounts.AnyAsync(x => x.Id == hostedSupplierId && x.Status != HostedSupplierStatus.Suspended, ct)) return Bad("hosted_supplier_invalid", "The hosted supplier is not active.");
        return null;
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
