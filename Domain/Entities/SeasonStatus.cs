namespace Edgias.MurimiOS.Domain.Entities;

public class SeasonStatus(string name, bool isDefault) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public bool IsDefault { get; private set; } = isDefault;

    public void Update(string name)
    {
        Name = name;
    }

    public void MakeDefault()
    {
        IsDefault = true;
    }
}


