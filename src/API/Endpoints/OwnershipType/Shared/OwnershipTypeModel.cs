using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared;

public record OwnershipTypeModel
{
    [Required]
    public string Name { get; set; } = default!;

}

