namespace EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared;

public record ItemCategoryResponse : ItemCategoryModel
{
    public Guid Id { get; set; }
}
