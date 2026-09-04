namespace EdgIQ.MurimiOS.Domain.OwnershipType;

public class OwnershipType(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }

}


