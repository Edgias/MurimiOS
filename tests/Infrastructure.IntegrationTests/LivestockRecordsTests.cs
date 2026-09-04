using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using EdgIQ.MurimiOS.Domain.LivestockTreatment;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockFeedBatch;

using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockExchangeSubmission;
using EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;
using EdgIQ.MurimiOS.Domain.LivestockMilkRecord;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using EdgIQ.MurimiOS.Domain.CropFeedTransfer;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using EdgIQ.MurimiOS.Domain.LivestockDispatch;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using LivestockDispatchPurpose = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatchPurpose;
using LivestockDispatch = global::EdgIQ.MurimiOS.Domain.LivestockDispatch.LivestockDispatch;
using LivestockFeedBatch = global::EdgIQ.MurimiOS.Domain.LivestockFeedBatch.LivestockFeedBatch;
using LivestockGroupMembership = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockGroupMembership;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class LivestockRecordsTests
{
    [Fact]
    public void Temporal_group_membership_can_only_end_once_and_after_joining()
    {
        DateTimeOffset joined = DateTimeOffset.UtcNow;
        var membership = new LivestockGroupMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), joined);
        Assert.Throws<ArgumentOutOfRangeException>(() => membership.Leave(joined.AddMinutes(-1)));
        membership.Leave(joined.AddDays(1));
        Assert.Equal(joined.AddDays(1), membership.LeftAt);
        Assert.Throws<InvalidOperationException>(() => membership.Leave(joined.AddDays(2)));
    }

    [Fact]
    public void Movement_lifecycle_requires_valid_holding_transitions()
    {
        Guid subject = Guid.NewGuid();
        Guid holding = Guid.NewGuid();
        DateTimeOffset at = DateTimeOffset.UtcNow;
        var birth = new LivestockMovementRecord(Guid.NewGuid(), subject, LivestockMovementType.Birth,
            null, holding, at, "BIRTH-1", null, null);
        Assert.Equal(holding, birth.ToHoldingId);
        Assert.Throws<ArgumentException>(() => new LivestockMovementRecord(Guid.NewGuid(), subject,
            LivestockMovementType.Transfer, holding, holding, at, null, null, null));
        Assert.Throws<ArgumentException>(() => new LivestockMovementRecord(Guid.NewGuid(), subject,
            LivestockMovementType.Death, null, null, at, null, null, null));
    }

    [Fact]
    public void Feed_batches_use_canonical_units_and_cannot_be_overissued()
    {
        var batch = new LivestockFeedBatch(Guid.NewGuid(), Guid.NewGuid(), "FEED-1", "Grower ration",
            1.5m, "t", DateOnly.FromDateTime(DateTime.UtcNow), null, "Supplier");
        Assert.Equal(1500m, batch.InitialQuantity);
        Assert.Equal("kg", batch.Unit);
        batch.Issue(250m);
        Assert.Equal(1250m, batch.QuantityOnHand);
        Assert.Throws<InvalidOperationException>(() => batch.Issue(1251m));
    }

    [Fact]
    public void Treatment_dispatch_sale_uses_the_meat_withdrawal_channel()
    {
        Assert.Equal(4, (int)LivestockDispatchPurpose.Sale);
        var blocked = Assert.Throws<InvalidOperationException>(() => new LivestockDispatch(Guid.NewGuid(),
            Guid.NewGuid(), LivestockDispatchPurpose.Sale, DateTimeOffset.UtcNow, "Buyer",
            DateTimeOffset.UtcNow.AddDays(2), null, null));
        Assert.Contains("withdrawal", blocked.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Completed_livestock_history_is_append_only_but_membership_can_close()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(Guid.NewGuid()));
        var movement = new LivestockMovementRecord(Guid.NewGuid(), Guid.NewGuid(), LivestockMovementType.Birth,
            null, Guid.NewGuid(), DateTimeOffset.UtcNow, null, null, null);
        var membership = new LivestockGroupMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTimeOffset.UtcNow);
        db.LivestockMovements.Add(movement); db.LivestockGroupMemberships.Add(membership);
        await db.SaveChangesAsync();

        membership.Leave(DateTimeOffset.UtcNow.AddHours(1));
        await db.SaveChangesAsync();
        Assert.NotNull(membership.LeftAt);

        db.LivestockMovements.Remove(movement);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}
