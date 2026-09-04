namespace EdgIQ.MurimiOS.Domain.EquipmentCategory;

public class EquipmentCategory(string name) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public void Update(string name)
    {
        Name = name;
    }

}


