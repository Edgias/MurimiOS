using System.ComponentModel.DataAnnotations;

namespace Edgias.MurimiOS.API.Loans;

public record LoanResponse : LoanModel
{
    [Required]
    public Guid Id { get; set; }

    public string CurrencyName { get; set; } = default!;

}

