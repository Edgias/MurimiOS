using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockMilkRecord;

public sealed class LivestockMilkRecord : TenantEntity
{
    private LivestockMilkRecord() { }
    public LivestockMilkRecord(Guid id, Guid farmId, Guid subjectId, DateTimeOffset milkedAt, MilkSession session,
        decimal quantity, decimal discardedQuantity, string unit, DateTimeOffset? withdrawalHoldUntil,
        decimal? fatPercent, decimal? proteinPercent, string? qualityGrade, string? notes) : base(id)
    {
        if (farmId == Guid.Empty || subjectId == Guid.Empty) throw new ArgumentException("Farm and livestock subject are required.");
        if (!Enum.IsDefined(session) || quantity <= 0 || discardedQuantity < 0 || discardedQuantity > quantity)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        MeasuredQuantity total = MeasurementUnits.Capture(quantity, unit, unit);
        MeasuredQuantity discarded = MeasurementUnits.Capture(discardedQuantity, unit, unit);
        if (total.Dimension != UnitDimension.Volume) throw new ArgumentException("Milk must use a volume unit.");
        if (withdrawalHoldUntil > milkedAt && discarded.CanonicalValue != total.CanonicalValue)
            throw new InvalidOperationException("Milk produced during a withdrawal hold must be fully withheld.");
        ValidatePercent(fatPercent); ValidatePercent(proteinPercent);
        FarmId = farmId; SubjectId = subjectId; MilkedAt = milkedAt.ToUniversalTime(); Session = session;
        QuantityLitres = total.CanonicalValue; DiscardedLitres = discarded.CanonicalValue; SaleableLitres = total.CanonicalValue - discarded.CanonicalValue;
        EnteredQuantity = total.EnteredValue; EnteredDiscardedQuantity = discarded.EnteredValue; EnteredUnit = total.EnteredUnit;
        WithdrawalHoldUntil = withdrawalHoldUntil?.ToUniversalTime(); FatPercent = fatPercent; ProteinPercent = proteinPercent;
        QualityGrade = Optional(qualityGrade, 80); Notes = Optional(notes, 2_000);
    }
    public Guid FarmId { get; private set; }
    public Guid SubjectId { get; private set; }
    public DateTimeOffset MilkedAt { get; private set; }
    public MilkSession Session { get; private set; }
    public decimal QuantityLitres { get; private set; }
    public decimal DiscardedLitres { get; private set; }
    public decimal SaleableLitres { get; private set; }
    public decimal EnteredQuantity { get; private set; }
    public decimal EnteredDiscardedQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public DateTimeOffset? WithdrawalHoldUntil { get; private set; }
    public decimal? FatPercent { get; private set; }
    public decimal? ProteinPercent { get; private set; }
    public string? QualityGrade { get; private set; }
    public string? Notes { get; private set; }
    private static void ValidatePercent(decimal? value) { if (value is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(value)); }
    private static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Value cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
