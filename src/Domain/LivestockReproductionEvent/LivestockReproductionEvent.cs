using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;

public sealed class LivestockReproductionEvent : TenantEntity
{
    private LivestockReproductionEvent() { }
    public LivestockReproductionEvent(Guid id, Guid farmId, Guid subjectId, ReproductionEventType type,
        ReproductionOutcome outcome, DateTimeOffset occurredAt, Guid? relatedSubjectId, Guid? breedingEventId,
        string? method, DateOnly? expectedDueOn, int? offspringCount, decimal? weight, string? weightUnit,
        string? practitioner, string? notes) : base(id)
    {
        if (farmId == Guid.Empty || subjectId == Guid.Empty) throw new ArgumentException("Farm and livestock subject are required.");
        if (!Enum.IsDefined(type) || !Enum.IsDefined(outcome)) throw new ArgumentOutOfRangeException(nameof(type));
        if (relatedSubjectId == Guid.Empty || breedingEventId == Guid.Empty || relatedSubjectId == subjectId || breedingEventId == id)
            throw new ArgumentException("Related identifiers must be distinct and non-empty.");
        Validate(type, outcome, expectedDueOn, offspringCount);
        if (expectedDueOn < DateOnly.FromDateTime(occurredAt.UtcDateTime))
            throw new ArgumentException("Expected due date cannot precede the diagnosis.", nameof(expectedDueOn));
        FarmId = farmId; SubjectId = subjectId; Type = type; Outcome = outcome; OccurredAt = occurredAt.ToUniversalTime();
        RelatedSubjectId = relatedSubjectId; BreedingEventId = breedingEventId; Method = Optional(method, 120);
        ExpectedDueOn = expectedDueOn; OffspringCount = offspringCount; Practitioner = Optional(practitioner, 200); Notes = Optional(notes, 2_000);
        if (weight.HasValue)
        {
            if (weight <= 0 || string.IsNullOrWhiteSpace(weightUnit)) throw new ArgumentException("A positive weaning/birth weight and unit are required together.");
            MeasuredQuantity captured = MeasurementUnits.Capture(weight.Value, weightUnit, weightUnit);
            if (captured.Dimension != UnitDimension.Mass) throw new ArgumentException("Reproduction weights must use a mass unit.");
            WeightKg = captured.CanonicalValue; EnteredWeight = captured.EnteredValue; EnteredWeightUnit = captured.EnteredUnit;
        }
        else if (!string.IsNullOrWhiteSpace(weightUnit)) throw new ArgumentException("A weight unit requires a weight value.");
    }
    public Guid FarmId { get; private set; }
    public Guid SubjectId { get; private set; }
    public ReproductionEventType Type { get; private set; }
    public ReproductionOutcome Outcome { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public Guid? RelatedSubjectId { get; private set; }
    public Guid? BreedingEventId { get; private set; }
    public string? Method { get; private set; }
    public DateOnly? ExpectedDueOn { get; private set; }
    public int? OffspringCount { get; private set; }
    public decimal? WeightKg { get; private set; }
    public decimal? EnteredWeight { get; private set; }
    public string? EnteredWeightUnit { get; private set; }
    public string? Practitioner { get; private set; }
    public string? Notes { get; private set; }
    private static void Validate(ReproductionEventType type, ReproductionOutcome outcome, DateOnly? due, int? count)
    {
        bool valid = type switch
        {
            ReproductionEventType.Breeding => outcome == ReproductionOutcome.Performed && due is null && count is null,
            ReproductionEventType.PregnancyDiagnosis => outcome is ReproductionOutcome.Pregnant or ReproductionOutcome.Open or ReproductionOutcome.Inconclusive && count is null && (outcome == ReproductionOutcome.Pregnant || due is null),
            ReproductionEventType.Parturition => outcome is ReproductionOutcome.LiveBirth or ReproductionOutcome.Stillbirth or ReproductionOutcome.MixedBirth or ReproductionOutcome.Abortion && due is null && count is >= 0,
            ReproductionEventType.Weaning => outcome == ReproductionOutcome.Weaned && due is null && count is null,
            _ => false
        };
        if (!valid) throw new ArgumentException("The reproduction outcome and details do not match the event type.");
    }
    private static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
