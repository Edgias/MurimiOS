namespace Edgias.MurimiOS.Domain.Entities.LivestockProductionAggregate;

public class Livestock(string name, LivestockType livestockType, string breed,
    Gender gender, string labels, string internalId, LivestockStatus livestockStatus,
    bool isNeutered, bool isBreedingStock, string color, decimal retentionScore,
    string description, string tagNumber, string tagColor, TagLocation tagLocation,
    string otherTagNumber, string otherTagColor, TagLocation otherTagLocation,
    string electronicId, string registryNumber, DateTime dateOfBirth, Guid? maternityId,
    Guid? paternityId, decimal birthWeight, decimal ageToWean, DateTime? dateWeaned,
    DateTime datePurchased, decimal? purchasePrice, Guid? purchasedFromId,
    Guid? breederId, Guid? veterinarianId, bool isOnFeed, string feedType,
    LivestockUnitOfMeasure harvestMeasure, decimal? estimatedRevenue, decimal? estimatedValue)
    : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = name;

    public LivestockType LivestockType { get; private set; } = livestockType;

    public string Breed { get; private set; } = breed;

    public Gender Gender { get; private set; } = gender;

    public string Labels { get; private set; } = labels;

    public string InternalId { get; private set; } = internalId;

    public LivestockStatus LivestockStatus { get; private set; } = livestockStatus;

    public bool IsNeutered { get; private set; } = isNeutered;

    public bool IsBreedingStock { get; private set; } = isBreedingStock;

    public string Color { get; private set; } = color;

    public decimal RetentionScore { get; private set; } = retentionScore;

    public string Description { get; private set; } = description;

    public string TagNumber { get; private set; } = tagNumber;

    public string TagColor { get; private set; } = tagColor;

    public TagLocation TagLocation { get; private set; } = tagLocation;

    public string OtherTagNumber { get; private set; } = otherTagNumber;

    public string OtherTagColor { get; private set; } = otherTagColor;

    public TagLocation OtherTagLocation { get; private set; } = otherTagLocation;

    public string ElectronicId { get; private set; } = electronicId;

    public string RegistryNumber { get; private set; } = registryNumber;

    public DateTime? DateOfBirth { get; private set; } = dateOfBirth;

    public Guid? MaternityId { get; private set; } = maternityId;

    public Livestock? Maternity { get; private set; }

    public Guid? PaternityId { get; private set; } = paternityId;

    public Livestock? Paternity { get; private set; }

    public decimal? BirthWeight { get; private set; } = birthWeight;

    public decimal AgeToWean { get; private set; } = ageToWean;

    public DateTime? DateWeaned { get; private set; } = dateWeaned;

    public DateTime? DatePurchased { get; private set; } = datePurchased;

    public decimal? PurchasePrice { get; private set; } = purchasePrice;

    public Guid? PurchasedFromId { get; private set; } = purchasedFromId;

    public Contact? PurchasedFrom { get; private set; }

    public Guid? BreederId { get; private set; } = breederId;

    public Contact? Breeder { get; private set; }

    public Guid? VeterinarianId { get; private set; } = veterinarianId;

    public Contact? Veterinarian { get; private set; }

    public bool IsOnFeed { get; private set; } = isOnFeed;

    public string FeedType { get; private set; } = feedType;

    public LivestockUnitOfMeasure HarvestMeasure { get; private set; } = harvestMeasure;

    public decimal? EstimatedRevenue { get; private set; } = estimatedRevenue;

    public decimal? EstimatedValue { get; private set; } = estimatedValue;

    private readonly List<LivestockFeeding> _feedings = [];
    public IReadOnlyCollection<LivestockFeeding> Feedings => _feedings.AsReadOnly();

    private readonly List<LivestockGrazing> _grazings = [];
    public IReadOnlyCollection<LivestockGrazing> Grazings => _grazings.AsReadOnly();

    private readonly List<LivestockInput> _inputs = [];
    public IReadOnlyCollection<LivestockInput> Inputs => _inputs.AsReadOnly();

    private readonly List<LivestockMeasurement> _measurements = [];
    public IReadOnlyCollection<LivestockMeasurement> Measurements => _measurements.AsReadOnly();

    private readonly List<LivestockPhoto> _photos = [];
    public IReadOnlyCollection<LivestockPhoto> Photos => _photos.AsReadOnly();

    private readonly List<LivestockTreatment> _treatments = [];
    public IReadOnlyCollection<LivestockTreatment> Treatments => _treatments.AsReadOnly();

    public void Update(string name, LivestockType livestockType, string breed,
    Gender gender, string labels, string internalId,
    bool isNeutered, bool isBreedingStock, string color, decimal retentionScore,
    string description, string tagNumber, string tagColor, TagLocation tagLocation,
    string otherTagNumber, string otherTagColor, TagLocation otherTagLocation,
    string electronicId, string registryNumber, DateTime dateOfBirth, Guid? maternityId,
    Guid? paternityId, decimal birthWeight, decimal ageToWean, DateTime? dateWeaned,
    DateTime datePurchased, decimal? purchasePrice, Guid? purchasedFromId,
    Guid? breederId, Guid? veterinarianId, bool isOnFeed, string feedType,
    LivestockUnitOfMeasure harvestMeasure, decimal? estimatedRevenue, decimal? estimatedValue)
    {
        Name = name;
        LivestockType = livestockType;
        Breed = breed;
        Gender = gender;
        Labels = labels;
        InternalId = internalId;
        IsNeutered = isNeutered;
        IsBreedingStock = isBreedingStock;
        Color = color;
        RetentionScore = retentionScore;
        Description = description;
        TagNumber = tagNumber;
        TagColor = tagColor;
        TagLocation = tagLocation;
        OtherTagNumber = otherTagNumber;
        OtherTagColor = otherTagColor;
        OtherTagLocation = otherTagLocation;
        ElectronicId = electronicId;
        RegistryNumber = registryNumber;
        DateOfBirth = dateOfBirth;
        MaternityId = maternityId;
        PaternityId = paternityId;
        BirthWeight = birthWeight;
        AgeToWean = ageToWean;
        DateWeaned = dateWeaned;
        DatePurchased = datePurchased;
        PurchasePrice = purchasePrice;
        PurchasedFromId = purchasedFromId;
        BreederId = breederId;
        VeterinarianId = veterinarianId;
        IsOnFeed = isOnFeed;
        FeedType = feedType;
        HarvestMeasure = harvestMeasure;
        EstimatedRevenue = estimatedRevenue;
        EstimatedValue = estimatedValue;
    }

    public void AddFeeding(LivestockFeeding feeding)
    {
        _feedings.Add(feeding);
    }

    public void AddFeedings(IEnumerable<LivestockFeeding> feedings)
    {
        _feedings.AddRange(feedings);
    }

    public void AddGrazing(LivestockGrazing grazing)
    {
        _grazings.Add(grazing);
    }

    public void AddGrazings(IEnumerable<LivestockGrazing> grazings)
    {
        _grazings.AddRange(grazings);
    }

    public void AddInput(LivestockInput input)
    {
        _inputs.Add(input);
    }

    public void AddInputs(IEnumerable<LivestockInput> inputs)
    {
        _inputs.AddRange(inputs);
    }

    public void AddMeasurement(LivestockMeasurement measurement)
    {
        _measurements.Add(measurement);
    }

    public void AddMeasurements(IEnumerable<LivestockMeasurement> measurements)
    {
        _measurements.AddRange(measurements);
    }

    public void AddPhoto(LivestockPhoto photo)
    {
        _photos.Add(photo);
    }

    public void AddPhotos(IEnumerable<LivestockPhoto> photos)
    {
        _photos.AddRange(photos);
    }

    public void AddTreatment(LivestockTreatment treatment)
    {
        _treatments.Add(treatment);
    }

    public void AddTreatments(IEnumerable<LivestockTreatment> treatments)
    {
        _treatments.AddRange(treatments);
    }

    public void ChangeStatus(LivestockStatus livestockStatus)
    {
        LivestockStatus = livestockStatus;
    }
}


