using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using EdgIQ.MurimiOS.Domain.SupplierSelfServiceToken;
using EdgIQ.MurimiOS.Domain.SupplyContract;
using EdgIQ.MurimiOS.Domain.HostedSupplierAccount;
using EdgIQ.MurimiOS.Domain.SupplierInvitation;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.DataGrant;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using CrossTenantAccessAudit = global::EdgIQ.MurimiOS.Domain.DataGrant.CrossTenantAccessAudit;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using SupplyDataCategories = global::EdgIQ.MurimiOS.Domain.DataGrant.SupplyDataCategories;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class SupplyNetworkTests
{
    [Fact]
    public void Relationship_request_requires_counterparty_acceptance_and_supports_revocation()
    {
        Guid producer = Guid.NewGuid();
        Guid buyer = Guid.NewGuid();
        DateTimeOffset requestedAt = DateTimeOffset.UtcNow;
        var relationship = new SupplyRelationship(Guid.NewGuid(), producer, buyer, producer,
            "2026 maize sourcing", requestedAt);

        Assert.Throws<InvalidOperationException>(() => relationship.Accept(producer, requestedAt.AddMinutes(1)));
        relationship.Accept(buyer, requestedAt.AddMinutes(2));
        Assert.Equal(SupplyRelationshipStatus.Active, relationship.Status);
        Assert.Equal(2u, relationship.Version);

        relationship.Revoke(producer, requestedAt.AddDays(1), "Contract concluded.");
        Assert.Equal(SupplyRelationshipStatus.Revoked, relationship.Status);
        Assert.Equal(producer, relationship.RevokedByTenantId);
        Assert.Equal(3u, relationship.Version);
    }

    [Fact]
    public void Grant_enforces_recipient_purpose_category_time_and_immediate_revocation()
    {
        Guid producer = Guid.NewGuid();
        Guid buyer = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;
        var grant = new DataGrant(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), producer, buyer,
            "Harvest visibility", "contract-sourcing-2026",
            [SupplyDataCategories.HarvestLots, SupplyDataCategories.Traceability],
            ["lotCode", "crop", "quantity", "unit"], [Guid.NewGuid()], [], ["Maize"],
            now.AddHours(-1), now.AddDays(30), false, 1, null, now.AddHours(-1));

        Assert.True(grant.IsEffective(buyer, "contract-sourcing-2026", SupplyDataCategories.HarvestLots, now));
        Assert.False(grant.IsEffective(producer, "contract-sourcing-2026", SupplyDataCategories.HarvestLots, now));
        Assert.False(grant.IsEffective(buyer, "benchmarking", SupplyDataCategories.HarvestLots, now));
        Assert.False(grant.IsEffective(buyer, "contract-sourcing-2026", SupplyDataCategories.Evidence, now));
        Assert.False(grant.IsEffective(buyer, "contract-sourcing-2026", SupplyDataCategories.HarvestLots, now.AddDays(31)));

        grant.Revoke(producer, now, "Producer withdrew consent.");
        Assert.False(grant.IsEffective(buyer, "contract-sourcing-2026", SupplyDataCategories.HarvestLots, now));
        Assert.Throws<InvalidOperationException>(() => grant.Revoke(producer, now, "Again"));
    }

    [Fact]
    public void Grant_definitions_validate_scope_categories_and_version_lineage_inputs()
    {
        Guid producer = Guid.NewGuid();
        Guid buyer = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;

        Assert.Throws<ArgumentException>(() => new DataGrant(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), producer, buyer,
            "Grant", "purpose", ["unsupported"], ["crop"], [Guid.NewGuid()], [], [], now, null, false, 1, null, now));
        Assert.Throws<ArgumentException>(() => new DataGrant(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), producer, buyer,
            "Grant", "purpose", [SupplyDataCategories.HarvestLots], ["crop"], [], [], [], now, null, false, 1, null, now));
        Assert.Throws<ArgumentException>(() => new DataGrant(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), producer, buyer,
            "Grant", "purpose", [SupplyDataCategories.HarvestLots], ["crop"], [Guid.NewGuid()], [], [], now,
            now.AddMinutes(-1), false, 1, null, now));
    }

    [Fact]
    public async Task Shared_access_audit_is_append_only_in_the_application_model()
    {
        Guid recipient = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(recipient));
        var audit = new CrossTenantAccessAudit(Guid.NewGuid(), Guid.NewGuid(), recipient, Guid.NewGuid(), recipient,
            "buyer-user", "contract-sourcing-2026", SupplyDataCategories.HarvestLots, null,
            DateTimeOffset.UtcNow, 4, "Allowed", "trace-1");
        db.CrossTenantAccessAudits.Add(audit);
        await db.SaveChangesAsync();

        db.CrossTenantAccessAudits.Remove(audit);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Only_producer_management_roles_can_issue_data_grants()
    {
        Assert.Contains(PlatformPermissions.DataGrantManage, PlatformPermissions.ForRole(TenantRole.Owner));
        Assert.Contains(PlatformPermissions.DataGrantManage, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.DoesNotContain(PlatformPermissions.DataGrantManage, PlatformPermissions.ForRole(TenantRole.Buyer));
        Assert.Contains(PlatformPermissions.SupplyNetworkRead, PlatformPermissions.ForRole(TenantRole.Buyer));
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}
