using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.CostCentre;

public sealed class CostCentre : TenantEntity
{
    private CostCentre() { }

    public CostCentre(Guid id, Guid farmId, string code, string name, CostCentreType type,
        Guid? targetId, string currency) : base(id)
    {
        if (farmId == Guid.Empty || targetId == Guid.Empty)
            throw new ArgumentException("A farm is required and an optional target must be non-empty.");
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (type == CostCentreType.Overhead && targetId is not null)
            throw new ArgumentException("An overhead cost centre cannot reference a production target.", nameof(targetId));
        if (type != CostCentreType.Overhead && targetId is null)
            throw new ArgumentException("A non-overhead cost centre requires its farm, field, crop-zone, or livestock target.", nameof(targetId));
        FarmId = farmId;
        Code = Required(code, nameof(code), 80).ToUpperInvariant();
        Name = Required(name, nameof(name), 200);
        Type = type;
        TargetId = targetId;
        Currency = new CurrencyCode(currency).Value;
        IsActive = true;
    }

    public Guid FarmId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public CostCentreType Type { get; private set; }
    public Guid? TargetId { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public string? ClosedBy { get; private set; }

    public void Close(DateTimeOffset at, string actor)
    {
        if (!IsActive) throw new InvalidOperationException("The cost centre is already closed.");
        IsActive = false;
        ClosedAt = at.ToUniversalTime();
        ClosedBy = Required(actor, nameof(actor), 240);
    }

    private static string Required(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum)
            throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }
}
