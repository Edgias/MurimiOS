using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.FarmField;

public sealed class FarmField : TenantEntity
{
    private FarmField()
    {
    }

    public FarmField(Guid id, Guid farmId, string name) : base(id)
    {
        if (farmId == Guid.Empty)
        {
            throw new ArgumentException("A farm is required.", nameof(farmId));
        }

        string normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length is 0 or > 200)
        {
            throw new ArgumentException("A field name between 1 and 200 characters is required.", nameof(name));
        }

        FarmId = farmId;
        Name = normalizedName;
    }

    public Guid FarmId { get; private set; }

    public string Name { get; private set; } = string.Empty;
}
