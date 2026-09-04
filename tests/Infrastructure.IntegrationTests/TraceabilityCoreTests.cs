using EdgIQ.MurimiOS.Domain.TenantMembership;
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

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using DeliveryStatus = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryStatus;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using RecallCase = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallCase;
using RecallStatus = global::EdgIQ.MurimiOS.Domain.RecallCase.RecallStatus;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
using TraceabilityMassBalance = global::EdgIQ.MurimiOS.Domain.StorageLot.TraceabilityMassBalance;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;
using TraceRelationshipType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceRelationshipType;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class TraceabilityCoreTests
{
    [Fact]
    public void Storage_lots_preserve_entered_quantity_and_use_canonical_balance_unit()
    {
        var lot = new StorageLot(Guid.NewGuid(), Guid.NewGuid(), "PACK-001", "Maize", "SC 727",
            1.25m, "t", DateTimeOffset.UtcNow, "Silo 2");

        Assert.Equal(1250m, lot.Quantity);
        Assert.Equal("kg", lot.Unit);
        Assert.Equal(1.25m, lot.EnteredQuantity);
        Assert.Equal("t", lot.EnteredUnit);
    }

    [Fact]
    public void Transformation_mass_balance_accepts_loss_and_rejects_unbalanced_outputs()
    {
        TraceabilityMassBalance.EnsureBalanced(1000m, 985m, 15m);

        InvalidOperationException error = Assert.Throws<InvalidOperationException>(
            () => TraceabilityMassBalance.EnsureBalanced(1000m, 990m, 5m));
        Assert.Contains("Mass balance failed", error.Message);
    }

    [Fact]
    public void Trace_links_reject_self_references()
    {
        Guid objectId = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => new TraceLink(Guid.NewGuid(), Guid.NewGuid(),
            TraceObjectType.HarvestLot, objectId, TraceObjectType.HarvestLot, objectId,
            TraceRelationshipType.TransformedInto));
    }

    [Fact]
    public void Delivery_and_recall_state_transitions_enforce_chronology()
    {
        DateTimeOffset dispatchedAt = DateTimeOffset.UtcNow;
        var delivery = new Delivery(Guid.NewGuid(), Guid.NewGuid(), "DEL-001", "Buyer", "Harare",
            dispatchedAt, "PO-123");
        Assert.Throws<ArgumentOutOfRangeException>(() => delivery.MarkReceived(dispatchedAt.AddMinutes(-1)));
        delivery.MarkReceived(dispatchedAt.AddHours(2));
        Assert.Equal(DeliveryStatus.Received, delivery.Status);
        Assert.Throws<InvalidOperationException>(() => delivery.MarkReceived(dispatchedAt.AddHours(3)));

        var recall = new RecallCase(Guid.NewGuid(), "REC-001", "Failed residue test", dispatchedAt);
        recall.Close(dispatchedAt.AddHours(1));
        Assert.Equal(RecallStatus.Closed, recall.Status);
    }

    [Fact]
    public async Task Trace_ledger_rows_are_append_only_in_the_context()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var traceEvent = new TraceEvent(Guid.NewGuid(), Guid.NewGuid(), TraceEventType.Object,
            "harvesting", "harvested", DateTimeOffset.UtcNow, "crop_zone", Guid.NewGuid(),
            "HarvestLot", Guid.NewGuid(), null);
        db.TraceEvents.Add(traceEvent);
        await db.SaveChangesAsync();

        db.TraceEvents.Remove(traceEvent);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Operational_roles_receive_traceability_permissions()
    {
        Assert.Contains(PlatformPermissions.TraceabilityWrite, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.RecallManage, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.TraceabilityRead, PlatformPermissions.ForRole(TenantRole.Buyer));
        Assert.DoesNotContain(PlatformPermissions.TraceabilityWrite, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public bool IsResolved => true;
        public Guid TenantId { get; } = tenantId;
    }
}
