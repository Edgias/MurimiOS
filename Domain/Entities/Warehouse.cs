namespace Edgias.MurimiOS.Domain.Entities;

public class Warehouse(string name, Guid locationId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public Guid LocationId { get; private set; } = locationId;

    public Location Location { get; private set; } = default!;

    public void Update(string name, Guid locationId)
    {
        Name = name;
        LocationId = locationId;
    }

}


