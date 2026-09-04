namespace EdgIQ.MurimiOS.Domain.SeasonStatus;

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


