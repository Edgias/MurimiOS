using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared;

public record EquipmentModel
{
    [Required]
    public string Name { get; set; } = default!;

    public string Manufacturer { get; set; } = default!;

    public string Model { get; set; } = default!;

    public string RegistrationNumber { get; set; } = default!;

    public int Year { get; set; } 

    public string Description { get; set; } = default!;

    public Guid EquipmentCategoryId { get; set; } 

}

