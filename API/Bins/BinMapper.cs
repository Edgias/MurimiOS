namespace Edgias.MurimiOS.API.Bins;

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

