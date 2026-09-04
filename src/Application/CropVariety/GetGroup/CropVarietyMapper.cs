namespace Edgias.MurimiOS.API.CropVarieties;

public static class CropVarietyMapper
{
    public static CropVarietyResponse AsApiResponse(this CropVariety entity)
    {
        CropVarietyResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            CropId = entity.CropId,
            CropName = entity.Crop?.Name!
        };

        return response;
    }

    public static CropVariety ToEntity(this CropVarietyRequest request)
    {
        CropVariety entity = new(request.Name, request.CropId);

        return entity;
    }

    public static void Update(this CropVariety entity, CropVarietyRequest request)
    {
        entity.Update(request.Name);
    }
}

