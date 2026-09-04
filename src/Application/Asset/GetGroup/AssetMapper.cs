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
using EdgIQ.MurimiOS.Domain.Supplier;
using EdgIQ.MurimiOS.Domain.Asset;
using EdgIQ.MurimiOS.Domain.PurchaseInvoice;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Asset.GetGroup;
using Asset = global::EdgIQ.MurimiOS.Domain.Asset.Asset;
using PurchaseInvoice = global::EdgIQ.MurimiOS.Domain.PurchaseInvoice.PurchaseInvoice;
using Supplier = global::EdgIQ.MurimiOS.Domain.Supplier.Supplier;

public static class AssetMapper
{
    public static AssetResponse AsApiResponse(this Asset entity)
    {
        AssetResponse dto = new()
        {
            Id = entity.Id,
            Name = entity.Name,
            SerialNumber = entity.SerialNumber,
            Description = entity.Description,
            PurchaseDate = entity.PurchaseDate,
            PurchaseInvoiceId = entity.PurchaseInvoiceId,
            PurchaseInvoiceNumber = entity.PurchaseInvoice?.Name!,
            PurchasePrice = entity.PurchasePrice,
            SupplierId = entity.SupplierId,
            SupplierName = entity.Supplier?.Name!
        };

        return dto;
    }

    public static Asset ToEntity(this AssetRequest request)
    {
        Asset entity = new(request.Name, request.SerialNumber, request.Description, request.PurchaseDate,
            request.PurchasePrice, request.SupplierId, request.PurchaseInvoiceId);

        return entity;
    }

    public static void Update(this Asset entity, AssetRequest request)
    {
        entity.Update(request.Name, request.SerialNumber, request.Description, request.PurchaseDate, request.PurchasePrice);
    }
}
