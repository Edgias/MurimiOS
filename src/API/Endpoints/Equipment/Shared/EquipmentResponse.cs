namespace EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared;

public record EquipmentResponse : EquipmentModel
{
    public Guid Id { get; set; }
}
