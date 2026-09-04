namespace Edgias.MurimiOS.API.CropCategories;

public static class CropCategoryMapper
{
    public static CropCategoryResponse AsApiResponse(this CropCategory entity)
    {
        CropCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };

        return response;
    }

    public static CropCategory ToEntity(this CropCategoryRequest request) 
    {
        CropCategory entity = new(request.Name, request.Description);

        return entity;
    }

    public static void Update(this CropCategory entity, CropCategoryRequest request)
    {
        entity.Update(request.Name, request.Description);
    }
}
