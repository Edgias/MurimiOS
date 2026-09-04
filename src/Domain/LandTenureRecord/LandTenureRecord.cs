using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.LandTenureRecord;

public sealed class LandTenureRecord : TenantEntity
{
    private LandTenureRecord() { }
    public LandTenureRecord(Guid id, Guid farmId, Guid? fieldId, LandTenureType type, string holderName,
        string? counterpartyName, string reference, decimal? registeredAreaHa, DateOnly validFrom,
        DateOnly? validTo, string? documentReference, string? notes) : base(id)
    {
        if (farmId == Guid.Empty || fieldId == Guid.Empty || !Enum.IsDefined(type)
            || registeredAreaHa <= 0 || validTo < validFrom) throw new ArgumentException("Land-tenure scope, type, area, or dates are invalid.");
        FarmId = farmId; FieldId = fieldId; Type = type; HolderName = DigitalTwinText.Required(holderName, 240);
        CounterpartyName = DigitalTwinText.Optional(counterpartyName, 240); Reference = DigitalTwinText.Required(reference, 160);
        RegisteredAreaHa = registeredAreaHa; ValidFrom = validFrom; ValidTo = validTo;
        DocumentReference = DigitalTwinText.Optional(documentReference, 500); Notes = DigitalTwinText.Optional(notes, 2_000);
        Status = validTo.HasValue ? LandTenureStatus.Ended : LandTenureStatus.Active;
    }
    public Guid FarmId { get; private set; }
    public Guid? FieldId { get; private set; }
    public LandTenureType Type { get; private set; }
    public string HolderName { get; private set; } = "";
    public string? CounterpartyName { get; private set; }
    public string Reference { get; private set; } = "";
    public decimal? RegisteredAreaHa { get; private set; }
    public DateOnly ValidFrom { get; private set; }
    public DateOnly? ValidTo { get; private set; }
    public LandTenureStatus Status { get; private set; }
    public string? DocumentReference { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }
    public void Close(DateOnly validTo, LandTenureStatus outcome, DateTimeOffset at, string actor)
    {
        if (Status != LandTenureStatus.Active || outcome is LandTenureStatus.Active || !Enum.IsDefined(outcome)
            || validTo < ValidFrom) throw new InvalidOperationException("The tenure period cannot be closed with the requested outcome or date.");
        ValidTo = validTo; Status = outcome; ClosedAt = at.ToUniversalTime(); ClosedBy = DigitalTwinText.Required(actor, 240);
    }
}
