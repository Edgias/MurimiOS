namespace EdgIQ.MurimiOS.Domain.CropProduction;
using CropVariety = global::EdgIQ.MurimiOS.Domain.CropVariety.CropVariety;

public class CropProductionVariety(Guid cropProductionId, Guid cropVarientyId) : BaseEntity
{
    public Guid CropProductionId { get; private set; } = cropProductionId;

    public CropProduction CropProduction { get; private set; } = default!;

    public Guid CropVarietyId { get; private set; } = cropVarientyId;

    public CropVariety CropVariety { get; private set; } = default!;
}


