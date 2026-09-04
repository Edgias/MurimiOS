using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockReproductionEvent;

public sealed class LivestockOffspringLink : TenantEntity
{
    private LivestockOffspringLink() { }
    public LivestockOffspringLink(Guid id, Guid parturitionEventId, Guid offspringSubjectId, OffspringOutcome outcome,
        decimal? birthWeight, string? birthWeightUnit) : base(id)
    {
        if (parturitionEventId == Guid.Empty || offspringSubjectId == Guid.Empty) throw new ArgumentException("Parturition and offspring are required.");
        if (!Enum.IsDefined(outcome)) throw new ArgumentOutOfRangeException(nameof(outcome));
        ParturitionEventId = parturitionEventId; OffspringSubjectId = offspringSubjectId; Outcome = outcome;
        if (birthWeight.HasValue)
        {
            if (birthWeight <= 0 || string.IsNullOrWhiteSpace(birthWeightUnit)) throw new ArgumentException("A positive birth weight and unit are required together.");
            MeasuredQuantity captured = MeasurementUnits.Capture(birthWeight.Value, birthWeightUnit, birthWeightUnit);
            if (captured.Dimension != UnitDimension.Mass) throw new ArgumentException("Birth weight must use a mass unit.");
            BirthWeightKg = captured.CanonicalValue; EnteredBirthWeight = captured.EnteredValue; EnteredBirthWeightUnit = captured.EnteredUnit;
        }
        else if (!string.IsNullOrWhiteSpace(birthWeightUnit)) throw new ArgumentException("A birth-weight unit requires a weight value.");
    }
    public Guid ParturitionEventId { get; private set; }
    public Guid OffspringSubjectId { get; private set; }
    public OffspringOutcome Outcome { get; private set; }
    public decimal? BirthWeightKg { get; private set; }
    public decimal? EnteredBirthWeight { get; private set; }
    public string? EnteredBirthWeightUnit { get; private set; }
}
