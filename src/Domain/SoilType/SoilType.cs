namespace EdgIQ.MurimiOS.Domain.SoilType;

public class SoilType(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }

}


