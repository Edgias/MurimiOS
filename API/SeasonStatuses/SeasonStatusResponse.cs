using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.SeasonStatuses;

public record SeasonStatusResponse : SeasonStatusModel
{
    [Required]
    public Guid Id { get; set; }

}

