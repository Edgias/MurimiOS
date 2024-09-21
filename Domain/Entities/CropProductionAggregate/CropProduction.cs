namespace Edgias.MurimiOS.Domain.Entities.CropProductionAggregate;

public class CropProduction : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;

    public Guid CropId { get; private set; }

    public Crop Crop { get; private set; } = default!;

    public Guid SeasonId { get; private set; }

    public Season Season { get; private set; } = default!;

    public decimal ExpectedYield { get; private set; }

    public decimal? ActualYield { get; private set; }

    private readonly List<CropProductionField> _cropProductionFields = [];

    private readonly List<CropProductionVariety> _cropProductionVarieties = [];

    public IReadOnlyCollection<CropProductionField> CropProductionFields => 
        _cropProductionFields.AsReadOnly();

    public IReadOnlyCollection<CropProductionVariety> CropProductionVarieties => 
        _cropProductionVarieties.AsReadOnly();

    private CropProduction()
    {
        // Required by EF
    }

    public CropProduction(string name, Guid cropId, Guid seasonId, decimal expectedYield,
        decimal? actualYield)
    {
        Name = name;
        CropId = cropId;
        SeasonId = seasonId;
        ExpectedYield = expectedYield;
        ActualYield = actualYield;
    }

    public CropProduction(string name, Guid cropId, Guid seasonId, decimal expectedYield, 
        decimal? actualYield,
        List<CropProductionField> cropProductionFields, 
        List<CropProductionVariety> cropProductionVarieties)
    {
        Name = name;
        CropId = cropId;
        SeasonId = seasonId;
        ExpectedYield = expectedYield;
        ActualYield = actualYield;
        _cropProductionFields = cropProductionFields;
        _cropProductionVarieties = cropProductionVarieties;
    }

    public void AddCropProductionField(CropProductionField cropProductionField)
    {
        Guard.AgainstNull(cropProductionField, nameof(cropProductionField));

        _cropProductionFields.Add(cropProductionField);
    }

    public void AddCropProductionVariety(CropProductionVariety cropProductionVariety)
    {
        _cropProductionVarieties.Add(cropProductionVariety);
    }

    public void UpdateDetails(string name, decimal expectedYield, decimal? actualYield)
    {
        Name = name;
        ExpectedYield = expectedYield;
        ActualYield = actualYield;
    }
}
