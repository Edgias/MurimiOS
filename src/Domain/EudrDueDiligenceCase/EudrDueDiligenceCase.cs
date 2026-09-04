using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;

public sealed class EudrDueDiligenceCase : TenantEntity
{
    private EudrDueDiligenceCase() { }

    public EudrDueDiligenceCase(Guid id, Guid farmId, string caseNumber, EudrCommodity commodity,
        string productDescription, string hsCode, decimal quantity, string unit, string productionCountry,
        DateOnly productionFrom, DateOnly productionTo, string operatorName, string operatorRegistrationNumber,
        string operatorAddress, string market, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        if (!Enum.IsDefined(commodity)) throw new ArgumentOutOfRangeException(nameof(commodity));
        if (productionTo < productionFrom) throw new ArgumentException("Production end cannot precede its start.");
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("DDS quantities must use mass, volume, or count units.", nameof(unit));
        string country = Text(productionCountry, 2).ToUpperInvariant();
        if (country.Length != 2) throw new ArgumentException("Production country must be a two-letter code.", nameof(productionCountry));

        FarmId = farmId;
        CaseNumber = Text(caseNumber, 80);
        Commodity = commodity;
        ProductDescription = Text(productDescription, 500);
        HsCode = Text(hsCode, 20);
        Quantity = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        ProductionCountry = country;
        ProductionFrom = productionFrom;
        ProductionTo = productionTo;
        OperatorName = Text(operatorName, 240);
        OperatorRegistrationNumber = Text(operatorRegistrationNumber, 120);
        OperatorAddress = Text(operatorAddress, 500);
        Market = Text(market, 120);
        CreatedAt = createdAt.ToUniversalTime();
        CreatedBy = Text(createdBy, 240);
        Status = EudrCaseStatus.Draft;
    }

    public Guid FarmId { get; private set; }
    public string CaseNumber { get; private set; } = string.Empty;
    public EudrCommodity Commodity { get; private set; }
    public string ProductDescription { get; private set; } = string.Empty;
    public string HsCode { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public string ProductionCountry { get; private set; } = string.Empty;
    public DateOnly ProductionFrom { get; private set; }
    public DateOnly ProductionTo { get; private set; }
    public string OperatorName { get; private set; } = string.Empty;
    public string OperatorRegistrationNumber { get; private set; } = string.Empty;
    public string OperatorAddress { get; private set; } = string.Empty;
    public string Market { get; private set; } = string.Empty;
    public EudrCaseStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTimeOffset? AssessedAt { get; private set; }
    public DateTimeOffset? ReadyAt { get; private set; }
    public DateTimeOffset? ExportedAt { get; private set; }
    public DateTimeOffset? SubmittedAt { get; private set; }
    public string? SubmissionReference { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    public void StartScreening()
    {
        if (Status == EudrCaseStatus.Draft) Status = EudrCaseStatus.Screening;
        else if (Status != EudrCaseStatus.Screening) throw new InvalidOperationException("Only a draft or screening case can accept screening evidence.");
    }

    public void RecordAssessment(DateTimeOffset at)
    {
        if (Status != EudrCaseStatus.Screening) throw new InvalidOperationException("A case must be screening before it can be assessed.");
        Status = EudrCaseStatus.Assessed;
        AssessedAt = at.ToUniversalTime();
    }

    public void MarkReady(DateTimeOffset at)
    {
        if (Status != EudrCaseStatus.Assessed) throw new InvalidOperationException("Only an assessed case can assemble a DDS.");
        Status = EudrCaseStatus.Ready;
        ReadyAt = at.ToUniversalTime();
    }

    public void MarkExported(DateTimeOffset at)
    {
        if (Status is not (EudrCaseStatus.Ready or EudrCaseStatus.Exported))
            throw new InvalidOperationException("Only a ready DDS can be exported.");
        Status = EudrCaseStatus.Exported;
        ExportedAt ??= at.ToUniversalTime();
    }

    public void MarkSubmitted(DateTimeOffset at, string reference)
    {
        if (Status != EudrCaseStatus.Exported) throw new InvalidOperationException("Export the DDS before recording submission.");
        Status = EudrCaseStatus.Submitted;
        SubmittedAt = at.ToUniversalTime();
        SubmissionReference = Text(reference, 200);
    }

    public void Cancel(DateTimeOffset at, string reason)
    {
        if (Status is EudrCaseStatus.Submitted or EudrCaseStatus.Cancelled)
            throw new InvalidOperationException("A submitted or cancelled case cannot be cancelled.");
        Status = EudrCaseStatus.Cancelled;
        CancelledAt = at.ToUniversalTime();
        CancellationReason = Text(reason, 2_000);
    }

    internal static string Text(string? value, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum)
            throw new ArgumentException($"Text must contain between 1 and {maximum} characters.");
        return result;
    }

    internal static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Text cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }

    internal static string Sha256(string value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? string.Empty;
        if (result.Length != 64 || result.Any(character => !Uri.IsHexDigit(character)))
            throw new ArgumentException("A hexadecimal SHA-256 digest is required.");
        return result;
    }
}
