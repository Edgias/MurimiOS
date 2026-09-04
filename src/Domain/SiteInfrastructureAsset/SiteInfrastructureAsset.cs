using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;

public sealed class SiteInfrastructureAsset : TenantEntity
{
    private SiteInfrastructureAsset() { }
    public SiteInfrastructureAsset(Guid id, Guid farmId, Guid? fieldId, string code, string name,
        InfrastructureAssetType type, InfrastructureAssetStatus status, Point? location, DateOnly? installedOn,
        decimal? capacity, string? capacityUnit, string detailsJson) : base(id)
    {
        if (farmId == Guid.Empty || fieldId == Guid.Empty || !Enum.IsDefined(type) || !Enum.IsDefined(status)
            || status == InfrastructureAssetStatus.Decommissioned || capacity <= 0
            || capacity.HasValue != !string.IsNullOrWhiteSpace(capacityUnit))
            throw new ArgumentException("Infrastructure scope, type, status, or capacity is invalid.");
        if (location is not null && (location.IsEmpty || !location.IsValid || location.X is < -180 or > 180 || location.Y is < -90 or > 90))
            throw new ArgumentException("Infrastructure location must be a valid WGS84 point.");
        FarmId = farmId; FieldId = fieldId; Code = DigitalTwinText.Required(code, 80).ToUpperInvariant();
        Name = DigitalTwinText.Required(name, 200); Type = type; Status = status; Location = location;
        if (Location is not null) Location.SRID = 4326;
        InstalledOn = installedOn; Capacity = capacity; CapacityUnit = DigitalTwinText.Optional(capacityUnit, 30);
        DetailsJson = DigitalTwinText.JsonObject(detailsJson, 20_000);
    }
    public Guid FarmId { get; private set; }
    public Guid? FieldId { get; private set; }
    public string Code { get; private set; } = "";
    public string Name { get; private set; } = "";
    public InfrastructureAssetType Type { get; private set; }
    public InfrastructureAssetStatus Status { get; private set; }
    public Point? Location { get; private set; }
    public DateOnly? InstalledOn { get; private set; }
    public decimal? Capacity { get; private set; }
    public string? CapacityUnit { get; private set; }
    public string DetailsJson { get; private set; } = "{}";
    public DateOnly? DecommissionedOn { get; private set; }
    public DateTimeOffset? DecommissionedAt { get; private set; }
    public string? DecommissionedBy { get; private set; }
    public string? DecommissionReason { get; private set; }
    public void SetOperationalStatus(InfrastructureAssetStatus status)
    {
        if (Status == InfrastructureAssetStatus.Decommissioned || status == InfrastructureAssetStatus.Decommissioned || !Enum.IsDefined(status))
            throw new InvalidOperationException("Use decommissioning for the terminal infrastructure transition.");
        Status = status;
    }
    public void Decommission(DateOnly on, DateTimeOffset at, string actor, string reason)
    {
        if (Status == InfrastructureAssetStatus.Decommissioned || InstalledOn > on)
            throw new InvalidOperationException("The infrastructure asset cannot be decommissioned on that date.");
        Status = InfrastructureAssetStatus.Decommissioned; DecommissionedOn = on; DecommissionedAt = at.ToUniversalTime();
        DecommissionedBy = DigitalTwinText.Required(actor, 240); DecommissionReason = DigitalTwinText.Required(reason, 2_000);
    }
}
