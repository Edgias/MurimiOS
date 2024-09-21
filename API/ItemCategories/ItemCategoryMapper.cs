namespace Edgias.MurimiOS.API.ItemCategories;

public static class ItemCategoryMapper
{
    public static ItemCategoryResponse AsApiResponse(this ItemCategory entity)
    {
        ItemCategoryResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static ItemCategory ToEntity(this ItemCategoryRequest request)
    {
        ItemCategory entity = new(request.Name);

        return entity;
    }

    public static void Update(this  ItemCategory entity, ItemCategoryRequest request) 
    {
        entity.Update(request.Name);
    }
}
