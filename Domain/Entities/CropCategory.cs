namespace Edgias.MurimiOS.Domain.Entities;

public class CropCategory(string name, string description) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public string Description { get; private set; } = description;

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }

}


