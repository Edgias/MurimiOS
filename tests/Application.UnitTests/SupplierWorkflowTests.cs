using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.DataGrant;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.SupplyNetwork;

namespace EdgIQ.MurimiOS.Application.UnitTests;
using HostedSupplierAccount = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierAccount;
using HostedSupplierStatus = global::EdgIQ.MurimiOS.Domain.HostedSupplierAccount.HostedSupplierStatus;
using SupplierEvidenceRequest = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequest;
using SupplierEvidenceRequestStatus = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequestStatus;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;
using SupplierInvitationStatus = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitationStatus;
using SupplierRiskBand = global::EdgIQ.MurimiOS.Domain.SupplierRiskAssessment.SupplierRiskBand;
using SupplierSelfServiceAction = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceAction;
using SupplierSelfServiceToken = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceToken;
using SupplierSelfServiceTokenStatus = global::EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken.SupplierSelfServiceTokenStatus;
using ContractAllocation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocation;
using ContractAllocationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractAllocationStatus;
using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractPricingMethod = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPricingMethod;
using ContractReconciliation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliation;
using ContractReconciliationStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliationStatus;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
using SupplyContractStatus = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContractStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using ContractPriceQuote = global::EdgIQ.MurimiOS.Application.Common.SupplyNetwork.ContractPriceQuote;
using ContractPricing = global::EdgIQ.MurimiOS.Application.Common.SupplyNetwork.ContractPricing;
using SupplierRiskInputs = global::EdgIQ.MurimiOS.Application.Common.SupplyNetwork.SupplierRiskInputs;
using SupplierRiskResult = global::EdgIQ.MurimiOS.Application.Common.SupplyNetwork.SupplierRiskResult;
using SupplierRiskScoring = global::EdgIQ.MurimiOS.Application.Common.SupplyNetwork.SupplierRiskScoring;

public sealed class SupplierWorkflowTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-30T12:00:00Z");

    [Fact]
    public void Invitation_token_is_hash_only_and_has_ordered_lifecycle()
    {
        string hash = new('a', 64);
        var invitation = new SupplierInvitation(Guid.NewGuid(), "Green Growers", "SUPPLIER@EXAMPLE.COM", hash,
            Now.AddDays(7), "buyer", Now);
        invitation.Accept(Guid.NewGuid(), Now.AddDays(1));
        Assert.Equal("supplier@example.com", invitation.Email);
        Assert.Equal(hash, invitation.TokenHash);
        Assert.Equal(SupplierInvitationStatus.Accepted, invitation.Status);
        Assert.Throws<InvalidOperationException>(() => invitation.Revoke(Now.AddDays(2)));
    }

    [Fact]
    public void Invitation_only_expires_after_its_deadline()
    {
        var invitation = new SupplierInvitation(Guid.NewGuid(), "Green Growers", "supplier@example.com",
            new string('b', 64), Now.AddDays(7), "buyer", Now);
        Assert.Throws<InvalidOperationException>(() => invitation.Expire(Now.AddDays(6)));
        invitation.Expire(Now.AddDays(7));
        Assert.Equal(SupplierInvitationStatus.Expired, invitation.Status);
    }

    [Fact]
    public void Contract_requires_exactly_one_supplier_target_and_price_currency_pair()
    {
        Assert.Throws<ArgumentException>(() => Contract(null, null));
        Assert.Throws<ArgumentException>(() => Contract(Guid.NewGuid(), Guid.NewGuid()));
        Assert.Throws<ArgumentException>(() => new SupplyContract(Guid.NewGuid(), "C-1", null, Guid.NewGuid(),
            "Maize", new DateOnly(2026, 9, 1), new DateOnly(2027, 1, 1), 100, "t", 200, null, null));
        SupplyContract contract = Contract(null, Guid.NewGuid());
        contract.Activate(Now); contract.Complete(Now.AddDays(1));
        Assert.Equal(SupplyContractStatus.Completed, contract.Status);
    }

    [Fact]
    public void Allocation_never_over_fulfils_committed_quantity()
    {
        var allocation = new ContractAllocation(Guid.NewGuid(), Guid.NewGuid(), "SEP-1",
            new DateOnly(2026, 9, 1), new DateOnly(2026, 9, 30), 100, "t");
        allocation.RecordFulfilment(40);
        Assert.Equal(ContractAllocationStatus.InProgress, allocation.Status);
        Assert.Throws<InvalidOperationException>(() => allocation.RecordFulfilment(61));
        allocation.RecordFulfilment(60);
        Assert.Equal(ContractAllocationStatus.Fulfilled, allocation.Status);
    }

    [Fact]
    public void Evidence_request_supports_rejection_and_resubmission()
    {
        var request = new SupplierEvidenceRequest(Guid.NewGuid(), null, Guid.NewGuid(), null, null, null,
            "GlobalGAP certificate", "Upload the current certificate", new DateOnly(2026, 9, 30));
        request.Submit(null, "supplier-record-123", Now);
        request.Reject(Now.AddHours(1), "Certificate expired");
        request.Submit(Guid.NewGuid(), null, Now.AddDays(1));
        request.Accept(Now.AddDays(1).AddHours(1), "Current certificate verified");
        Assert.Equal(SupplierEvidenceRequestStatus.Accepted, request.Status);
    }

    [Fact]
    public void Supplier_portal_management_is_available_to_buyer_but_not_viewer()
    {
        Assert.Contains(PlatformPermissions.SupplierPortalManage, PlatformPermissions.ForRole(TenantRole.Buyer));
        Assert.Contains(PlatformPermissions.SupplierPortalRead, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.DoesNotContain(PlatformPermissions.SupplierPortalManage, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    [Fact]
    public void Risk_scoring_is_reproducible_and_bands_material_delivery_and_evidence_failures()
    {
        SupplierRiskResult critical = SupplierRiskScoring.Calculate(new SupplierRiskInputs(
            EvidenceRequested: 2, EvidenceSubmitted: 0, EvidenceAccepted: 0, EvidenceRejected: 2,
            EvidenceOverdue: 4, PastDueAllocatedQuantity: 100, PastDueFulfilledQuantity: 0,
            ActiveContracts: 0, CompletedContracts: 0, TerminatedContracts: 2, AccountRiskScore: 100));
        SupplierRiskResult low = SupplierRiskScoring.Calculate(new SupplierRiskInputs(
            EvidenceRequested: 0, EvidenceSubmitted: 0, EvidenceAccepted: 4, EvidenceRejected: 0,
            EvidenceOverdue: 0, PastDueAllocatedQuantity: 100, PastDueFulfilledQuantity: 100,
            ActiveContracts: 0, CompletedContracts: 1, TerminatedContracts: 0, AccountRiskScore: 0));

        Assert.Equal(SupplierRiskBand.Critical, critical.Band);
        Assert.True(critical.Score >= 90);
        Assert.Equal(SupplierRiskBand.Low, low.Band);
        Assert.Equal(0, low.Score);
        Assert.Contains("supplier-risk-v1", SupplierRiskScoring.ModelVersion);
    }

    [Fact]
    public void Price_resolution_prefers_matching_quality_then_volume_and_calculates_server_amounts()
    {
        SupplyContract contract = Contract(null, Guid.NewGuid());
        DateOnly from = new(2026, 9, 1); DateOnly to = new(2027, 1, 1);
        ContractPriceRule[] rules =
        [
            new(Guid.NewGuid(), contract.Id, 3, ContractPricingMethod.Fixed, 0, null, null, 210, "USD", from, to),
            new(Guid.NewGuid(), contract.Id, 2, ContractPricingMethod.VolumeTier, 50, null, null, 195, "USD", from, to),
            new(Guid.NewGuid(), contract.Id, 1, ContractPricingMethod.QualityGrade, 0, null, "A", 225, "USD", from, to)
        ];

        ContractPriceQuote quality = ContractPricing.Resolve(contract, rules, 60, "A", from);
        ContractPriceQuote volume = ContractPricing.Resolve(contract, rules, 60, null, from);

        Assert.Equal(225, quality.UnitPrice);
        Assert.Equal(13_500, quality.GrossAmount);
        Assert.Equal(195, volume.UnitPrice);
    }

    [Fact]
    public void Reconciliation_enforces_quantity_and_amount_equations()
    {
        var reconciliation = new ContractReconciliation(Guid.NewGuid(), Guid.NewGuid(), 1, "REC-1", null,
            100, 90, 10, "t", 2, 180, -5, 2, 177, "USD", ContractReconciliationStatus.Exception,
            "Quality deduction", Now, "buyer");

        Assert.Equal(177, reconciliation.PayableAmount);
        Assert.Throws<ArgumentException>(() => new ContractReconciliation(Guid.NewGuid(), Guid.NewGuid(), 1,
            "REC-2", null, 100, 90, 5, "t", 2, 180, 0, 0, 180, "USD",
            ContractReconciliationStatus.Matched, null, Now, "buyer"));
    }

    [Fact]
    public void Hosted_supplier_self_service_token_is_single_use_and_detachment_is_tenant_bound()
    {
        Guid tenantId = Guid.NewGuid();
        var supplier = new HostedSupplierAccount(Guid.NewGuid(), "SUP-1", "Green Growers", "Jane",
            "jane@example.test", null);
        supplier.Claim(tenantId, Now);
        Assert.Throws<InvalidOperationException>(() => supplier.Detach(Guid.NewGuid(), Now.AddMinutes(1)));
        supplier.Detach(tenantId, Now.AddMinutes(1));
        Assert.Equal(HostedSupplierStatus.Active, supplier.Status);

        var token = new SupplierSelfServiceToken(Guid.NewGuid(), supplier.Id, SupplierSelfServiceAction.Claim,
            tenantId, new string('a', 64), Now.AddHours(1), Now, "buyer");
        token.Consume(Now.AddMinutes(5), new string('b', 64));
        Assert.Equal(SupplierSelfServiceTokenStatus.Consumed, token.Status);
        Assert.Throws<InvalidOperationException>(() => token.Consume(Now.AddMinutes(6), null));
    }

    private static SupplyContract Contract(Guid? relationshipId, Guid? hostedSupplierId) => new(Guid.NewGuid(), "C-1",
        relationshipId, hostedSupplierId, "Maize", new DateOnly(2026, 9, 1), new DateOnly(2027, 1, 1),
        100, "t", 200, "USD", "{}");
}
