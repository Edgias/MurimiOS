using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.EquipmentAsset;

public sealed class EquipmentAsset : TenantEntity
{
    private EquipmentAsset() { }
    public EquipmentAsset(Guid id, Guid farmId, string code, string name, EquipmentType type, string? manufacturer,
        string? model, string? serialNumber, int? modelYear) : base(id)
    {
        if (farmId == Guid.Empty || !Enum.IsDefined(type) || modelYear is < 1900 or > 2200) throw new ArgumentException("A valid farm, equipment type, and model year are required.");
        FarmId = farmId; Code = Text(code, 60).ToUpperInvariant(); Name = Text(name, 200); Type = type;
        Manufacturer = Optional(manufacturer, 120); Model = Optional(model, 120); SerialNumber = Optional(serialNumber, 160);
        ModelYear = modelYear; Status = EquipmentStatus.Active;
    }
    public Guid FarmId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public EquipmentType Type { get; private set; }
    public string? Manufacturer { get; private set; }
    public string? Model { get; private set; }
    public string? SerialNumber { get; private set; }
    public int? ModelYear { get; private set; }
    public EquipmentStatus Status { get; private set; }
    public DateTimeOffset? RetiredAt { get; private set; }
    public void SendToMaintenance() { if (Status != EquipmentStatus.Active) throw new InvalidOperationException("Only active equipment can enter maintenance."); Status = EquipmentStatus.Maintenance; }
    public void ReturnToService() { if (Status != EquipmentStatus.Maintenance) throw new InvalidOperationException("Only equipment under maintenance can return to service."); Status = EquipmentStatus.Active; }
    public void Retire(DateTimeOffset at) { if (Status == EquipmentStatus.Retired) throw new InvalidOperationException("The equipment is already retired."); Status = EquipmentStatus.Retired; RetiredAt = at.ToUniversalTime(); }
    internal static string Text(string? value, int max) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > max) throw new ArgumentException($"Text must contain between 1 and {max} characters."); return result; }
    internal static string? Optional(string? value, int max) { string? result = value?.Trim(); if (result?.Length > max) throw new ArgumentException($"Text cannot exceed {max} characters."); return string.IsNullOrEmpty(result) ? null : result; }
    internal static string Sha256(string? value) { string result = value?.Trim().ToLowerInvariant() ?? string.Empty; if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A hexadecimal SHA-256 digest is required."); return result; }
}
