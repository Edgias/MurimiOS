namespace Edgias.MurimiOS.Domain.Entities;

public class LivestockGroup(string name, string description)
        : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string Desciption { get; private set; } = description;

    public void Update(string name, string description)
    {
        Name = name;
        Desciption = description;
    }
}


