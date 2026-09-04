namespace Edgias.MurimiOS.API.SoilTypes;

public static class SoilTypeMapper
{
    public static SoilTypeResponse AsApiResponse(this SoilType entity)
    {
        SoilTypeResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static SoilType ToEntity(this SoilTypeRequest request)
    {
        SoilType entity = new(request.Name);

        return entity;
    }

    public static void Update(this SoilType entity, SoilTypeRequest request)
    {
        entity.Update(request.Name);
    }
}
