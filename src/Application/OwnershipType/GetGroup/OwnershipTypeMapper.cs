namespace Edgias.MurimiOS.API.OwnershipTypes;

public static class OwnershipTypeMapper
{
    public static OwnershipTypeResponse AsApiResponse(this OwnershipType entity)
    {
        OwnershipTypeResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static OwnershipType ToEntity(this OwnershipTypeRequest request)
    {
        OwnershipType entity = new(request.Name);

        return entity;
    }

    public static void Update(this OwnershipType entity, OwnershipTypeRequest request)
    {
        entity.Update(request.Name);
    }
}
