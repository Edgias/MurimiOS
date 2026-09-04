namespace EdgIQ.MurimiOS.Domain.CropProduction;
using Field = global::EdgIQ.MurimiOS.Domain.Field.Field;

public class CropProductionField(Guid fieldId, Guid cropProductionId) : BaseEntity
{
    public Guid FieldId { get; private set; } = fieldId;

    public Field Field { get; private set; } = default!;

    public Guid CropProductionId { get; private set; } = cropProductionId;

    public CropProduction CropProduction { get; private set; } = default!;
}


