using System.ComponentModel.DataAnnotations;
using Gender = global::EdgIQ.MurimiOS.Domain.LivestockProduction.Gender;
using ContactType = global::EdgIQ.MurimiOS.Domain.Contact.ContactType;
using LivestockType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockType;
using LivestockStatus = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockStatus;
using LivestockUnitOfMeasure = global::EdgIQ.MurimiOS.Domain.LivestockProduction.LivestockUnitOfMeasure;
using TagLocation = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TagLocation;
using TreatmentApplicationMethod = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentApplicationMethod;
using TreatmentType = global::EdgIQ.MurimiOS.Domain.LivestockProduction.TreatmentType;
using EdgIQ.MurimiOS.Domain.Warehouse;
using EdgIQ.MurimiOS.Domain.UnitMeasurement;
using EdgIQ.MurimiOS.Domain.BinType;
using EdgIQ.MurimiOS.Domain.Bins;
using EdgIQ.Dispatcher;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Application.Bins.PutGroupId;
using Bin = global::EdgIQ.MurimiOS.Domain.Bins.Bin;
using BinType = global::EdgIQ.MurimiOS.Domain.BinType.BinType;
using UnitMeasurement = global::EdgIQ.MurimiOS.Domain.UnitMeasurement.UnitMeasurement;
using Warehouse = global::EdgIQ.MurimiOS.Domain.Warehouse.Warehouse;


public static class BinMapper
{
    public static BinResponse AsApiResponse(this Bin entity)
    {
        BinResponse response = new()
        {
            Id = entity.Id,
            Capacity = entity.Capacity,
            Name = entity.Name,
            BinTypeId = entity.BinTypeId,
            BinTypeName = entity.BinType?.Name!,
            UnitGroupId = entity.UnitGroupId,
            UnitMeasurementId = entity.UnitMeasurementId,
            UnitMeasurementName = entity.UnitMeasurement.Name,
            WarehouseId = entity.WarehouseId,
            WarehouseName = entity.Warehouse?.Name!
        };

        return response;
    }

    public static Bin ToEntity(this BinRequest request)
    {
        Bin entity = new(request.Name, request.Capacity, request.BinTypeId, 
            request.UnitGroupId, request.UnitMeasurementId, request.WarehouseId);

        return entity;
    }

    public static void Update(this Bin entity, BinRequest request)
    {
        entity.Update(request.Name, request.Capacity);
    }
}
