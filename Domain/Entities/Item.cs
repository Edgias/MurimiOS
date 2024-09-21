namespace Edgias.MurimiOS.Domain.Entities;

public class Item(string name, string description, Guid itemCategoryId) : BaseEntity, IAggregateRoot
{
    public string Name { get; set; } = name;

    public string Description { get; set; } = description;

    public Guid ItemCategoryId { get; set; } = itemCategoryId;

    public ItemCategory ItemCategory { get; set; } = default!;

    public void Update(string name, string description, Guid itemCategoryId)
    {
        Name = name;
        Description = description;
        ItemCategoryId = itemCategoryId;
    }
}


