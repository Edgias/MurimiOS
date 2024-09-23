namespace Edgias.MurimiOS.API.Equipments;

public static class EquipmentMapper
{
    public static EquipmentResponse AsApiResponse(this Equipment entity)
    {
        EquipmentResponse response = new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return response;
    }

    public static Equipment ToEntity(this EquipmentRequest request)
    {
        Equipment entity = new(request.Name, request.Manufacturer, request.Model, request.RegistrationNumber, request.Year, request.Description, request.EquipmentCategoryId);

        return entity;
    }

    public static void Update(this Equipment entity, EquipmentRequest request)
    {
        entity.Update(request.Name, request.Manufacturer, request.Model, request.RegistrationNumber, request.Year, request.Description);
    }
}
