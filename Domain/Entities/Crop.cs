namespace Edgias.MurimiOS.Domain.Entities;

public class Crop(string name, Guid cropCategoryId, Guid cropUnitId) : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public Guid CropCategoryId { get; private set; } = cropCategoryId;

    public CropCategory CropCategory { get; private set; } = default!;

    public Guid CropUnitId { get; private set; } = cropUnitId;

    public CropUnit CropUnit { get; private set; } = default!;

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void UpdateCropCategory(Guid cropCategoryId)
    {
        CropCategoryId = cropCategoryId;
    }

    public void UpdateCropUnit(Guid cropUnitId)
    {
        CropUnitId = cropUnitId;
    }
}


