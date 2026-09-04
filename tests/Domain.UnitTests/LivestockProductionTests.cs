using EdgIQ.MurimiOS.Domain.TenantMembership;
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

namespace EdgIQ.MurimiOS.Domain.UnitTests;
using GrazingPlanStatus = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.GrazingPlanStatus;
using LivestockGrazingPlan = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.LivestockGrazingPlan;
using LivestockMilkRecord = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.LivestockMilkRecord;
using MilkSession = global::EdgIQ.MurimiOS.Domain.LivestockMilkRecord.MilkSession;
using LivestockRationLine = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationLine;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
using RationPlanStatus = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.RationPlanStatus;
using LivestockOffspringLink = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockOffspringLink;
using LivestockReproductionEvent = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.LivestockReproductionEvent;
using OffspringOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.OffspringOutcome;
using ReproductionEventType = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionEventType;
using ReproductionOutcome = global::EdgIQ.MurimiOS.Domain.LivestockReproductionEvent.ReproductionOutcome;
using LivestockBiologicalProfile = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockBiologicalProfile;
using LivestockSex = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSex;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;

public sealed class LivestockProductionTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-30T12:00:00Z");

    [Fact]
    public void Biological_profile_rejects_self_parentage()
    {
        Guid subjectId = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() => new LivestockBiologicalProfile(Guid.NewGuid(), subjectId,
            LivestockSex.Female, new DateOnly(2024, 1, 1), "Boran", subjectId, null));

        var profile = new LivestockBiologicalProfile(Guid.NewGuid(), subjectId, LivestockSex.Female,
            new DateOnly(2024, 1, 1), " Boran ", null, null);
        Assert.Equal("Boran", profile.Breed);
    }

    [Fact]
    public void Reproduction_event_enforces_type_specific_outcomes()
    {
        Assert.Throws<ArgumentException>(() => Event(ReproductionEventType.Breeding, ReproductionOutcome.Pregnant));
        LivestockReproductionEvent diagnosis = Event(ReproductionEventType.PregnancyDiagnosis, ReproductionOutcome.Pregnant,
            new DateOnly(2027, 1, 1));
        Assert.Equal(ReproductionOutcome.Pregnant, diagnosis.Outcome);
        Assert.Equal(new DateOnly(2027, 1, 1), diagnosis.ExpectedDueOn);
    }

    [Fact]
    public void Reproduction_weight_is_canonicalized_to_kilograms()
    {
        LivestockReproductionEvent weaning = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            ReproductionEventType.Weaning, ReproductionOutcome.Weaned, Now, null, null, null, null, null,
            220.462m, "lb", null, null);
        Assert.Equal(100.000m, decimal.Round(weaning.WeightKg!.Value, 3));
    }

    [Fact]
    public void Offspring_link_preserves_birth_outcome_and_weight()
    {
        var link = new LivestockOffspringLink(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            OffspringOutcome.Live, 35, "kg");
        Assert.Equal(35, link.BirthWeightKg);
        Assert.Equal(OffspringOutcome.Live, link.Outcome);
    }

    [Fact]
    public void Milk_under_withdrawal_must_be_fully_discarded()
    {
        Assert.Throws<InvalidOperationException>(() => new LivestockMilkRecord(Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Now, MilkSession.Morning, 10, 2, "L", Now.AddHours(6), null, null, null, null));

        var record = new LivestockMilkRecord(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Now,
            MilkSession.Morning, 10, 10, "L", Now.AddHours(6), 4.2m, 3.5m, "A", null);
        Assert.Equal(0, record.SaleableLitres);
        Assert.Equal(10, record.DiscardedLitres);
    }

    [Fact]
    public void Grazing_plan_lifecycle_is_ordered()
    {
        var plan = new LivestockGrazingPlan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Now, Now.AddDays(7), 20, Now.AddDays(30), 900, null, "manager");
        Assert.Throws<InvalidOperationException>(() => plan.Complete(Now));
        plan.Activate(); plan.Complete(Now.AddDays(7));
        Assert.Equal(GrazingPlanStatus.Completed, plan.Status);
    }

    [Fact]
    public void Ration_plan_and_lines_enforce_mass_and_lifecycle()
    {
        var plan = new LivestockRationPlan(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Grower",
            new DateOnly(2026, 9, 1), null, 10, null, "manager");
        var line = new LivestockRationLine(Guid.NewGuid(), plan.Id, null, "Silage", 12, "kg", 35);
        Assert.Equal(12, line.QuantityKgPerHeadPerDay);
        Assert.Throws<ArgumentException>(() => new LivestockRationLine(Guid.NewGuid(), plan.Id, null,
            "Water", 10, "L", 100));
        plan.Activate(Now); plan.Retire(Now.AddDays(1));
        Assert.Equal(RationPlanStatus.Retired, plan.Status);
    }

    [Fact]
    public void Livestock_permissions_keep_management_out_of_viewer_roles()
    {
        Assert.Contains(PlatformPermissions.LivestockWrite, PlatformPermissions.ForRole(TenantRole.LivestockManager));
        Assert.Contains(PlatformPermissions.LivestockRead, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.DoesNotContain(PlatformPermissions.LivestockWrite, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private static LivestockReproductionEvent Event(ReproductionEventType type, ReproductionOutcome outcome,
        DateOnly? due = null) => new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), type, outcome, Now,
            null, null, null, due, type == ReproductionEventType.Parturition ? 1 : null, null, null, null, null);
}
