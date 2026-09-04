using System.ComponentModel.DataAnnotations;

namespace EdgIQ.MurimiOS.API.Endpoints.Loan.Shared;

public record LoanResponse : LoanModel
{
    [Required]
    public Guid Id { get; set; }

    public string CurrencyName { get; set; } = default!;

}

