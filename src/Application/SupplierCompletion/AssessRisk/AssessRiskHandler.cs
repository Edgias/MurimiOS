using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Application.Common.SupplyNetwork;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplierCompletion.AssessRisk;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using HostedSupplierStatus = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplierEvidenceRequestStatus = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequestStatus;
using SupplierRiskAssessment = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskAssessment;
using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;
using ContractAllocationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocationStatus;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class AssessRiskHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock) : IRequestHandler<AssessRiskCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(AssessRiskCommand message, CancellationToken cancellationToken) => AssessRiskAsync(message.request, db, tenant, actor, context, clock, cancellationToken);

    private static async Task<UseCaseResult> AssessRiskAsync(AssessSupplierRiskRequest request, IPlatformDbContext db,
        ITenantContext tenant, IRequestActorContext actor, UseCaseContext context, TimeProvider clock, CancellationToken ct)
    {
        UseCaseResult? targetError = await ValidateTargetAsync(request.RelationshipId, request.HostedSupplierId, db, tenant, ct);
        if (targetError is not null) return targetError;
        DateTimeOffset now = clock.GetUtcNow();
        DateOnly today = DateOnly.FromDateTime(now.UtcDateTime);
        SupplyContract[] contracts = await db.SupplyContracts.AsNoTracking().Where(x =>
            request.RelationshipId.HasValue ? x.RelationshipId == request.RelationshipId : x.HostedSupplierId == request.HostedSupplierId).ToArrayAsync(ct);
        Guid[] contractIds = contracts.Select(x => x.Id).ToArray();
        ContractAllocation[] allocations = await db.ContractAllocations.AsNoTracking()
            .Where(x => contractIds.Contains(x.ContractId) && x.Status != ContractAllocationStatus.Cancelled && x.DeliveryTo < today).ToArrayAsync(ct);
        SupplierEvidenceRequest[] evidence = await db.SupplierEvidenceRequests.AsNoTracking().Where(x =>
            request.RelationshipId.HasValue ? x.RelationshipId == request.RelationshipId : x.HostedSupplierId == request.HostedSupplierId).ToArrayAsync(ct);
        int accountRisk;
        if (request.HostedSupplierId is { } supplierId)
        {
            HostedSupplierStatus status = await db.HostedSupplierAccounts.Where(x => x.Id == supplierId).Select(x => x.Status).SingleAsync(ct);
            accountRisk = status switch { HostedSupplierStatus.Suspended => 100, HostedSupplierStatus.Active => 15, _ => 0 };
        }
        else
        {
            SupplyRelationshipStatus status = await db.SupplyRelationships.Where(x => x.Id == request.RelationshipId).Select(x => x.Status).SingleAsync(ct);
            accountRisk = status == SupplyRelationshipStatus.Active ? 0 : 100;
        }
        var inputs = new SupplierRiskInputs(
            evidence.Count(x => x.Status == SupplierEvidenceRequestStatus.Requested),
            evidence.Count(x => x.Status == SupplierEvidenceRequestStatus.Submitted),
            evidence.Count(x => x.Status == SupplierEvidenceRequestStatus.Accepted),
            evidence.Count(x => x.Status == SupplierEvidenceRequestStatus.Rejected),
            evidence.Count(x => x.DueOn < today && x.Status is SupplierEvidenceRequestStatus.Requested or SupplierEvidenceRequestStatus.Rejected),
            allocations.Sum(x => x.AllocatedQuantity), allocations.Sum(x => x.FulfilledQuantity),
            contracts.Count(x => x.Status == SupplyContractStatus.Active),
            contracts.Count(x => x.Status == SupplyContractStatus.Completed),
            contracts.Count(x => x.Status == SupplyContractStatus.Terminated), accountRisk);
        SupplierRiskResult result = SupplierRiskScoring.Calculate(inputs);
        var assessment = new SupplierRiskAssessment(Guid.NewGuid(), request.RelationshipId, request.HostedSupplierId,
            result.Score, result.Band, SupplierRiskScoring.ModelVersion, now, result.ComponentsJson, result.InputsJson, now, actor.Subject);
        db.SupplierRiskAssessments.Add(assessment);
        AddRecords(db, tenant, actor.Subject, actor.MembershipId, actor.ScopeOrganisationNodeId, "RiskAssessed",
            assessment.Id, new { assessment.Id, assessment.RelationshipId, assessment.HostedSupplierId, assessment.Score, assessment.Band }, context, now);
        await db.SaveChangesAsync(ct);
        return UseCaseResults.Created($"/api/v1/supplier-portal/risk-assessments/{assessment.Id}", assessment);
    }


    private static async Task<UseCaseResult?> ValidateTargetAsync(Guid? relationshipId, Guid? hostedSupplierId,
        IPlatformDbContext db, ITenantContext tenant, CancellationToken ct)
    {
        if (relationshipId.HasValue == hostedSupplierId.HasValue) return Bad("supplier_target_invalid", "Choose one supplier target.");
        if (relationshipId.HasValue && !await db.SupplyRelationships.AnyAsync(x => x.Id == relationshipId
            && (x.ProducerTenantId == tenant.TenantId || x.BuyerTenantId == tenant.TenantId), ct))
            return Bad("relationship_invalid", "The supply relationship is not available.");
        if (hostedSupplierId.HasValue && !await db.HostedSupplierAccounts.AnyAsync(x => x.Id == hostedSupplierId, ct))
            return Bad("hosted_supplier_invalid", "The hosted supplier is not available.");
        return null;
    }


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, string actor,
        Guid? membershipId, Guid? scopeId, string action, Guid entityId, object payload,
        UseCaseContext context, DateTimeOffset now)
    {
        string json = JsonSerializer.Serialize(payload, JsonOptions);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor, membershipId,
            action, "SupplierWorkflow", entityId, null, scopeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, $"Supplier.{action}", json, now));
    }

    private static Guid Id(Guid? id) => id is { } value && value != Guid.Empty ? value : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

}
