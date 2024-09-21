namespace Edgias.MurimiOS.API.EquipmentCategories;

public static class EquipmentCategoryMapper
{
    public static EquipmentCategoryResponse AsApiResponse(this EquipmentCategory entity)
    {
        EquipmentCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static EquipmentCategory ToEntity(this EquipmentCategoryRequest request)
    {
        EquipmentCategory entity = new(request.Name);

        return entity;
    }

    public static void Update(this EquipmentCategory entity, EquipmentCategoryRequest request)
    {
        entity.Update(request.Name);
    }
}
