namespace Edgias.MurimiOS.API.BinTypes;

public static class BinTypeMapper
{
    public static BinTypeResponse AsApiResponse(this BinType entity)
    {
        BinTypeResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static BinType ToEntity(this BinTypeRequest request)
    {
        BinType entity = new(request.Name);

        return entity;
    }

    public static void Update(this BinType entity, BinTypeRequest request)
    {
        entity.Update(request.Name);
    }
}

