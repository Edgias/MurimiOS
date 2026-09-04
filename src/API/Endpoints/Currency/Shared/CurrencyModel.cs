using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Currencies;

public record CurrencyModel
{
    [Required]
    public string Name { get; set; } = default!;

    [Required]
    public string CurrencyCode { get; set; } = default!;

    [Required]
    public string Symbol { get; set; } = default!;

}

