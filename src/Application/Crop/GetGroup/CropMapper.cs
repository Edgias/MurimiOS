namespace Edgias.MurimiOS.API.Crops;

public static class CropMapper
{
    public static CropResponse AsApiResponse(this Crop entity)
    {
        CropResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            CropCategoryId = entity.CropCategoryId,
            CropCategoryName = entity.CropCategory?.Name!,
            CropUnitId = entity.CropUnitId,
            CropUnitName = entity.CropUnit?.Name!
        };

        return response;
    }

    public static Crop ToEntity(this CropRequest request)
    {
        Crop entity = new(request.Name, request.CropCategoryId, request.CropUnitId);

        return entity;
    }

    public static void Update(this Crop entity, CropRequest request)
    {
        entity.UpdateName(request.Name);
    }
}

