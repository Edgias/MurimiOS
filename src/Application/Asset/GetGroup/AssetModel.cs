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
namespace EdgIQ.MurimiOS.Application.Asset.GetGroup;
using Asset = global::EdgIQ.MurimiOS.Domain.Asset.Asset;
using PurchaseInvoice = global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.PurchaseInvoice;
using Supplier = global::EdgIQ.MurimiOS.Domain.Supplier.Supplier;

public record AssetModel
{
    [Required]
    public string Name { get; set; } = default!;

    public string SerialNumber { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime? PurchaseDate { get; set; }

    public decimal? PurchasePrice { get; set; }

    public Guid? SupplierId { get; set; }

    public Guid? PurchaseInvoiceId { get; set; }

}
