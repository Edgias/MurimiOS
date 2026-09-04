using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Loans;

public record LoanModel
{
    [Required]
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    [Required]
    public int Duration { get; set; }

    [Required]
    public decimal PrincipalAmount { get; set; }

    [Required]
    public decimal InterestRate { get; set; }

    public decimal EffectiveRate { get; set; }

    [Required]
    public Guid CurrencyId { get; set; }

}

