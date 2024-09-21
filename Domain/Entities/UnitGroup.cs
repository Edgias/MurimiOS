namespace Edgias.MurimiOS.Domain.Entities;

public class UnitGroup(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }

}


