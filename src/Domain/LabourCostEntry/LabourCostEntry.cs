using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.LabourCostEntry;

public sealed class LabourCostEntry : TenantEntity
{
    private LabourCostEntry() { }

    public LabourCostEntry(Guid id, Guid farmId, Guid transactionId, Guid costCentreId,
        Guid? fieldOperationId, Guid? cropZoneId, Guid? livestockSubjectId, string workerReference,
        DateTimeOffset occurredAt, decimal durationHours, decimal hourlyRate, string currency,
        string? notes) : base(id)
    {
        Validate(farmId, transactionId, costCentreId, fieldOperationId, cropZoneId, livestockSubjectId,
            durationHours, hourlyRate);
        FarmId = farmId; TransactionId = transactionId; CostCentreId = costCentreId;
        FieldOperationId = fieldOperationId; CropZoneId = cropZoneId; LivestockSubjectId = livestockSubjectId;
        WorkerReference = Required(workerReference, nameof(workerReference), 160);
        OccurredAt = occurredAt.ToUniversalTime(); EnteredDurationHours = durationHours;
        DurationMinutes = decimal.Round(durationHours * 60m, 4, MidpointRounding.AwayFromZero);
        HourlyRate = hourlyRate; Currency = new CurrencyCode(currency).Value;
        TotalCost = decimal.Round(durationHours * hourlyRate, 2, MidpointRounding.AwayFromZero);
        Notes = Optional(notes, 2_000);
    }

    public Guid FarmId { get; private set; }
    public Guid TransactionId { get; private set; }
    public Guid CostCentreId { get; private set; }
    public Guid? FieldOperationId { get; private set; }
    public Guid? CropZoneId { get; private set; }
    public Guid? LivestockSubjectId { get; private set; }
    public string WorkerReference { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public decimal DurationMinutes { get; private set; }
    public decimal EnteredDurationHours { get; private set; }
    public decimal HourlyRate { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal TotalCost { get; private set; }
    public string? Notes { get; private set; }

    private static void Validate(Guid farmId, Guid transactionId, Guid costCentreId, Guid? operationId,
        Guid? cropZoneId, Guid? livestockSubjectId, decimal hours, decimal rate)
    {
        if (farmId == Guid.Empty || transactionId == Guid.Empty || costCentreId == Guid.Empty
            || operationId == Guid.Empty || cropZoneId == Guid.Empty || livestockSubjectId == Guid.Empty)
            throw new ArgumentException("Farm, transaction and cost centre are required; optional identifiers must be non-empty.");
        if (cropZoneId is not null && livestockSubjectId is not null)
            throw new ArgumentException("A labour entry cannot target both a crop zone and livestock subject.");
        if (hours <= 0 || rate < 0) throw new ArgumentException("Duration must be positive and hourly rate cannot be negative.");
    }

    private static string Required(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
