namespace Edgias.MurimiOS.Domain.Entities.LivestockProductionAggregate;

public class LivestockGrazing(Guid locationId, DateTime startDate,
    DateTime? endDate) : BaseEntity
{
    public Guid LocationId { get; private set; } = locationId;

    public Location Location { get; private set; } = default!;

    public DateTime StartDate { get; private set; } = startDate;

    public DateTime? EndDate { get; private set; } = endDate;

    public Guid LivestockId { get; private set; }

    public Livestock Livestock { get; private set; } = default!;
}


