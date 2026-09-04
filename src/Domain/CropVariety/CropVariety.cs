namespace EdgIQ.MurimiOS.Domain.CropVariety;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;

public class CropVariety(string name, Guid cropId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public Guid CropId { get; private set; } = cropId;

    public Crop Crop { get; private set; } = default!;

    public void Update(string name)
    {
        Name = name;
    }
}


