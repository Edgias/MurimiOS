using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.MachineryCostEntry;

public sealed class MachineryCostEntry : TenantEntity
{
    private MachineryCostEntry() { }

    public MachineryCostEntry(Guid id, Guid farmId, Guid transactionId, Guid costCentreId,
        Guid? fieldOperationId, Guid? cropZoneId, Guid? livestockSubjectId, string machineReference,
        DateTimeOffset occurredAt, decimal durationHours, decimal hourlyRate, decimal additionalCost,
        string currency, string? notes) : base(id)
    {
        if (farmId == Guid.Empty || transactionId == Guid.Empty || costCentreId == Guid.Empty
            || fieldOperationId == Guid.Empty || cropZoneId == Guid.Empty || livestockSubjectId == Guid.Empty)
            throw new ArgumentException("Farm, transaction and cost centre are required; optional identifiers must be non-empty.");
        if (cropZoneId is not null && livestockSubjectId is not null)
            throw new ArgumentException("Machinery usage cannot target both a crop zone and livestock subject.");
        if (durationHours <= 0 || hourlyRate < 0 || additionalCost < 0)
            throw new ArgumentException("Duration must be positive and cost rates cannot be negative.");
        FarmId = farmId; TransactionId = transactionId; CostCentreId = costCentreId;
        FieldOperationId = fieldOperationId; CropZoneId = cropZoneId; LivestockSubjectId = livestockSubjectId;
        MachineReference = Required(machineReference, nameof(machineReference), 160);
        OccurredAt = occurredAt.ToUniversalTime(); EnteredDurationHours = durationHours;
        DurationMinutes = decimal.Round(durationHours * 60m, 4, MidpointRounding.AwayFromZero);
        HourlyRate = hourlyRate; AdditionalCost = decimal.Round(additionalCost, 2, MidpointRounding.AwayFromZero);
        Currency = new CurrencyCode(currency).Value;
        TotalCost = decimal.Round(durationHours * hourlyRate + additionalCost, 2, MidpointRounding.AwayFromZero);
        Notes = Optional(notes, 2_000);
    }

    public Guid FarmId { get; private set; }
    public Guid TransactionId { get; private set; }
    public Guid CostCentreId { get; private set; }
    public Guid? FieldOperationId { get; private set; }
    public Guid? CropZoneId { get; private set; }
    public Guid? LivestockSubjectId { get; private set; }
    public string MachineReference { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }
    public decimal DurationMinutes { get; private set; }
    public decimal EnteredDurationHours { get; private set; }
    public decimal HourlyRate { get; private set; }
    public decimal AdditionalCost { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public decimal TotalCost { get; private set; }
    public string? Notes { get; private set; }

    private static string Required(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
