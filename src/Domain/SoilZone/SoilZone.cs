using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SoilZone;

public sealed class SoilZone : TenantEntity
{
    private SoilZone() { }
    public SoilZone(Guid id, Guid farmId, Guid fieldId, string code, string name, string? classification,
        string? managementGuidance) : base(id)
    {
        if (farmId == Guid.Empty || fieldId == Guid.Empty) throw new ArgumentException("Farm and field are required.");
        FarmId = farmId; FieldId = fieldId; Code = DigitalTwinText.Required(code, 80).ToUpperInvariant();
        Name = DigitalTwinText.Required(name, 200); Classification = DigitalTwinText.Optional(classification, 120);
        ManagementGuidance = DigitalTwinText.Optional(managementGuidance, 4_000); Status = SoilZoneStatus.Active;
    }
    public Guid FarmId { get; private set; }
    public Guid FieldId { get; private set; }
    public string Code { get; private set; } = "";
    public string Name { get; private set; } = "";
    public string? Classification { get; private set; }
    public string? ManagementGuidance { get; private set; }
    public SoilZoneStatus Status { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public string? RetiredBy { get; private set; }
    public void Retire(DateTimeOffset at, string actor)
    {
        if (Status != SoilZoneStatus.Active) throw new InvalidOperationException("The soil zone is already retired.");
        Status = SoilZoneStatus.Retired; RetiredAt = at.ToUniversalTime(); RetiredBy = DigitalTwinText.Required(actor, 240);
    }
}
