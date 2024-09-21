namespace Edgias.MurimiOS.Domain.Entities.LivestockProductionAggregate;

public class LivestockPhoto(string name, byte[] content) : BaseEntity
{
    public string Name { get; private set; } = name;

    public byte[] Content { get; private set; } = content;

    public Guid LivestockId { get; private set; }

    public Livestock Livestock { get; private set; } = default!;
}
