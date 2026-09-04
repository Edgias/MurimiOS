using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.GrowingSeason;

public enum GrowingSeasonStatus
{
    Planned = 1,
    Active = 2,
    Closed = 3
}

public sealed class GrowingSeason : TenantEntity
{
    private GrowingSeason()
    {
    }

    public GrowingSeason(
        Guid id,
        Guid farmId,
        string name,
        DateOnly startsOn,
        DateOnly endsOn,
        GrowingSeasonStatus status) : base(id)
    {
        if (farmId == Guid.Empty)
        {
            throw new ArgumentException("A farm is required.", nameof(farmId));
        }

        string normalizedName = name?.Trim() ?? string.Empty;
        if (normalizedName.Length is 0 or > 120)
        {
            throw new ArgumentException("A season name between 1 and 120 characters is required.", nameof(name));
        }

        if (endsOn < startsOn)
        {
            throw new ArgumentException("The season end date cannot precede its start date.", nameof(endsOn));
        }

        if (!Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        FarmId = farmId;
        Name = normalizedName;
        StartsOn = startsOn;
        EndsOn = endsOn;
        Status = status;
    }

    public Guid FarmId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateOnly StartsOn { get; private set; }

    public DateOnly EndsOn { get; private set; }

    public GrowingSeasonStatus Status { get; private set; }
}
