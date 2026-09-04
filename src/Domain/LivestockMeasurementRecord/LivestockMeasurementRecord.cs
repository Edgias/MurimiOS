using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;

public sealed class LivestockMeasurementRecord : TenantEntity
{
    private LivestockMeasurementRecord()
    {
    }

    public LivestockMeasurementRecord(
        Guid id,
        Guid subjectId,
        string metric,
        decimal value,
        string unit,
        DateTimeOffset occurredAt,
        DateTimeOffset recordedAt,
        Guid sourceCommandId) : base(id)
    {
        if (subjectId == Guid.Empty || sourceCommandId == Guid.Empty)
        {
            throw new ArgumentException("A livestock subject and source command are required.");
        }

        SubjectId = subjectId;
        Metric = RequireText(metric, nameof(metric), 80);
        MeasuredQuantity measured = MeasurementUnits.Capture(value, unit, unit);
        Value = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredValue = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        DisplayUnit = measured.DisplayUnit;
        OccurredAt = occurredAt.ToUniversalTime();
        RecordedAt = recordedAt.ToUniversalTime();
        SourceCommandId = sourceCommandId;
    }

    public Guid SubjectId { get; private set; }

    public string Metric { get; private set; } = string.Empty;

    public decimal Value { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public decimal EnteredValue { get; private set; }

    public string EnteredUnit { get; private set; } = string.Empty;

    public string DisplayUnit { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset RecordedAt { get; private set; }

    public Guid SourceCommandId { get; private set; }

    public Guid? SupersededById { get; private set; }

    public string? CorrectionReason { get; private set; }

    public void Supersede(Guid replacementId, string reason)
    {
        if (replacementId == Guid.Empty || replacementId == Id)
            throw new ArgumentException("A distinct replacement measurement is required.", nameof(replacementId));
        if (SupersededById is not null)
            throw new InvalidOperationException("This livestock measurement has already been superseded.");
        string normalizedReason = reason?.Trim() ?? string.Empty;
        if (normalizedReason.Length is 0 or > 2_000)
            throw new ArgumentException("A correction reason between 1 and 2000 characters is required.", nameof(reason));
        SupersededById = replacementId;
        CorrectionReason = normalizedReason;
    }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        }

        return result;
    }
}
