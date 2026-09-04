namespace EdgIQ.MurimiOS.Domain.ItemCategory;

public class ItemCategory(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }
}


