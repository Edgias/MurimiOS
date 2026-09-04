using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockHolding;

public sealed class LivestockHolding : TenantEntity
{
    private LivestockHolding() { }
    public LivestockHolding(Guid id, Guid farmId, string code, string name, LivestockHoldingType type) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        FarmId = farmId; Code = Text(code, nameof(code), 80); Name = Text(name, nameof(name), 200); Type = type;
    }
    public Guid FarmId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public LivestockHoldingType Type { get; private set; }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}
