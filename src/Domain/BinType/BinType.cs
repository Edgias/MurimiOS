namespace EdgIQ.MurimiOS.Domain.BinType;

public class BinType(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }
}


