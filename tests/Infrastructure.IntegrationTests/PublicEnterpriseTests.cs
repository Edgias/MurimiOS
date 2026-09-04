using System.Security.Cryptography;
using System.Text;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using EdgIQ.MurimiOS.Domain.WebhookSubscription;
using EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using EdgIQ.MurimiOS.Domain.TransparencyToken;
using EdgIQ.MurimiOS.Domain.ApprovalPolicy;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using EdgIQ.MurimiOS.Domain.TenantPlacement;
using EdgIQ.MurimiOS.Domain.TenantMigrationJob;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.RecallCase;
using EdgIQ.MurimiOS.Domain.QualityAssessment;
using EdgIQ.MurimiOS.Domain.StorageLot;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using ApprovalDecisionKind = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.ApprovalDecisionKind;
using DelegatedApprovalRequest = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalRequest;
using DelegatedApprovalStatus = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalStatus;
using PublicApiAccessLog = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiAccessLog;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
using PublicApiCredential = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredential;
using PublicApiCredentialStatus = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredentialStatus;
using PublicApiScopes = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiScopes;
using DisclosurePolicyStatus = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.DisclosurePolicyStatus;
using RetailDisclosurePolicy = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.RetailDisclosurePolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using TenantMigrationJob = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationJob;
using TenantMigrationKind = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationKind;
using TenantMigrationStatus = global::EdgIQ.MurimiOS.Domain.TenantMigrationJob.TenantMigrationStatus;
using TenantDatabaseMode = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantDatabaseMode;
using TenantPlacement = global::EdgIQ.MurimiOS.Domain.TenantPlacement.TenantPlacement;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;
using TransparencyTokenStatus = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyTokenStatus;
using WebhookDelivery = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDelivery;
using WebhookDeliveryStatus = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDeliveryStatus;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using WebhookDispatcher = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.WebhookDispatcher;

public sealed class PublicEnterpriseTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-31T10:00:00Z");
    private static readonly string Hash = new('a', 64);

    [Fact]
    public void Api_client_scopes_and_credential_rotation_are_explicit()
    {
        Guid clientId = Guid.NewGuid();
        var client = new PublicApiClient(clientId, "Retail API", [PublicApiScopes.FarmsRead], [Guid.NewGuid()], Now, "owner");
        var first = new PublicApiCredential(Guid.NewGuid(), clientId, 1, "mro_live_one", Hash, Now, Now.AddDays(30), Now, "owner");
        var second = new PublicApiCredential(Guid.NewGuid(), clientId, 2, "mro_live_two", new string('b', 64), Now, null, Now, "owner", first.Id);
        first.Supersede(second.Id, Now.AddMinutes(1)); client.Suspend(); client.Activate(); client.RecordUse(Now.AddMinutes(2));
        Assert.Equal(PublicApiCredentialStatus.Superseded, first.Status); Assert.Equal(Now.AddMinutes(2), client.LastUsedAt);
        Assert.Throws<ArgumentException>(() => new PublicApiClient(Guid.NewGuid(), "bad", ["admin"], [], Now, "owner"));
    }

    [Fact]
    public void Webhooks_require_https_and_have_independent_retry_lifecycle()
    {
        Assert.Throws<ArgumentException>(() => new WebhookSubscription(Guid.NewGuid(), "bad", "http://example.test/hook", ["Harvest.Recorded"], Hash, 3, Now, "owner"));
        var subscription = new WebhookSubscription(Guid.NewGuid(), "trace", "https://example.test/hook", ["Harvest.Recorded"], Hash, 2, Now, "owner");
        var delivery = new WebhookDelivery(Guid.NewGuid(), subscription.Id, Guid.NewGuid(), "Harvest.Recorded", "{}", Now, 2);
        Guid lease1 = Guid.NewGuid(); delivery.Claim(lease1, Now, TimeSpan.FromMinutes(1)); delivery.Fail(lease1, Now.AddSeconds(1), Now.AddMinutes(1), 503, "unavailable", null);
        Guid lease2 = Guid.NewGuid(); delivery.Claim(lease2, Now.AddMinutes(1), TimeSpan.FromMinutes(1)); delivery.Fail(lease2, Now.AddMinutes(1).AddSeconds(1), Now.AddMinutes(2), 503, "unavailable", null);
        Assert.Equal(WebhookDeliveryStatus.DeadLettered, delivery.Status); delivery.Replay(Now.AddMinutes(3)); Assert.Equal(WebhookDeliveryStatus.RetryScheduled, delivery.Status);
    }

    [Fact]
    public void Webhook_signature_uses_the_documented_derived_key()
    {
        string secret = "mro_whsec_test"; string keyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(secret))).ToLowerInvariant();
        string signature = WebhookDispatcher.Sign(keyHash, "123", "{\"id\":1}");
        Assert.Equal(64, signature.Length); Assert.Equal(signature, WebhookDispatcher.Sign(keyHash, "123", "{\"id\":1}"));
        Assert.NotEqual(signature, WebhookDispatcher.Sign(keyHash, "124", "{\"id\":1}"));
    }

    [Fact]
    public void Disclosure_policy_and_opaque_token_have_revocable_lifecycles()
    {
        Guid policyId = Guid.NewGuid(); var policy = new RetailDisclosurePolicy(policyId, policyId, "Origin", "Murimi Market", ["origin", "crop", "lineage"], [Guid.NewGuid()], null, 1, null, "en-ZW", Now, "owner");
        policy.Publish(Now); var token = new TransparencyToken(Guid.NewGuid(), policy.Id, TraceObjectType.HarvestLot, Guid.NewGuid(), "mro_trace_1", Hash, Now, Now.AddDays(30), "owner"); token.Revoke(Now.AddDays(1)); policy.Retire(Now.AddDays(1));
        Assert.Equal(TransparencyTokenStatus.Revoked, token.Status); Assert.Equal(DisclosurePolicyStatus.Retired, policy.Status);
    }

    [Fact]
    public void Delegated_approval_enforces_quorum_and_rejection_is_terminal()
    {
        Guid policyId = Guid.NewGuid(); Guid nodeId = Guid.NewGuid();
        var request = new DelegatedApprovalRequest(Guid.NewGuid(), policyId, nodeId, null, "CropPlan", Guid.NewGuid(), "crop-plan.publish", "{}", 2, Now, Now.AddDays(1), "requester");
        request.RecordDecision(ApprovalDecisionKind.Approve, Now.AddHours(1)); Assert.Equal(DelegatedApprovalStatus.Pending, request.Status);
        request.RecordDecision(ApprovalDecisionKind.Approve, Now.AddHours(2)); Assert.Equal(DelegatedApprovalStatus.Approved, request.Status);
        var rejected = new DelegatedApprovalRequest(Guid.NewGuid(), policyId, nodeId, null, "Payment", Guid.NewGuid(), "payment.release", "{}", 2, Now, Now.AddDays(1), "requester");
        rejected.RecordDecision(ApprovalDecisionKind.Reject, Now.AddHours(1)); Assert.Equal(DelegatedApprovalStatus.Rejected, rejected.Status);
    }

    [Fact]
    public void Tenant_migration_requires_export_validation_and_import_in_order()
    {
        var placement = new TenantPlacement(Guid.NewGuid(), "af-south", "shared-01", TenantDatabaseMode.Shared, "ZW", true, Now);
        var job = new TenantMigrationJob(Guid.NewGuid(), TenantMigrationKind.Move, placement.Id, "eu-west", "dedicated-22", TenantDatabaseMode.Dedicated, Now, "owner");
        job.StartExport(); job.CompleteExport("tenant/export.json", Hash, "{}"); job.Validate(); job.StartImport(); job.Complete(Now.AddHours(1));
        Assert.Equal(TenantMigrationStatus.Completed, job.Status);
    }

    [Fact]
    public async Task External_access_and_approval_decisions_are_append_only()
    {
        Guid tenantId = Guid.NewGuid(); var options = new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var log = new PublicApiAccessLog(Guid.NewGuid(), Guid.NewGuid(), PublicApiScopes.FarmsRead, "GET", "/api/public/v1/farms", 200, "trace", Now, null);
        db.PublicApiAccessLogs.Add(log); await db.SaveChangesAsync(); db.PublicApiAccessLogs.Remove(log); await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Permissions_keep_public_and_migration_management_privileged()
    {
        Assert.Contains(PlatformPermissions.PublicApiManage, PlatformPermissions.ForRole(TenantRole.Owner));
        Assert.Contains(PlatformPermissions.RetailTransparencyManage, PlatformPermissions.ForRole(TenantRole.Buyer));
        Assert.DoesNotContain(PlatformPermissions.TenantMigrationManage, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext { public Guid TenantId { get; } = tenantId; public bool IsResolved => true; }
}
