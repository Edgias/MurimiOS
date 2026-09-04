using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Gender = global::EdgIQ.MurimiOS.Domain.LivestockProduction.Gender;
using ContactType = global::EdgIQ.MurimiOS.Domain.Contact.ContactType;
using LivestockType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockType;
using LivestockStatus = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockStatus;
using LivestockUnitOfMeasure = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockUnitOfMeasure;
using TagLocation = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TagLocation;
using TreatmentApplicationMethod = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentApplicationMethod;
using TreatmentType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentType;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Loan.GetGroup;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;
using Loan = global::EdgIQ.MurimiOS.Domain.Loan.Loan;

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
