using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared;

public record CropVarietyModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public Guid CropId { get; set; }
}

