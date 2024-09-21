namespace Edgias.MurimiOS.Domain.Entities.CropProductionAggregate;

public class CropProductionField(Guid fieldId, Guid cropProductionId) : BaseEntity
{
    public Guid FieldId { get; private set; } = fieldId;

    public Field Field { get; private set; } = default!;

    public Guid CropProductionId { get; private set; } = cropProductionId;

    public CropProduction CropProduction { get; private set; } = default!;
}


