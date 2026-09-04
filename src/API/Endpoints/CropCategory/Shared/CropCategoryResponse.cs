namespace EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared;

public record CropCategoryResponse : CropCategoryModel
{
    public Guid Id { get; set; }
}
