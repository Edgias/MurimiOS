namespace EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared;

public record FieldMeasurementResponse : FieldMeasurementModel
{
    public Guid Id { get; set; }
}
