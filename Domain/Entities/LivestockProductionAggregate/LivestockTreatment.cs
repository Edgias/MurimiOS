namespace Edgias.MurimiOS.Domain.Entities.LivestockProductionAggregate;

public class LivestockTreatment(TreatmentType treatmentType, string product, string batchNumber,
    string dosage, decimal? inventoryAmountUsed, LivestockUnitOfMeasure inventoryUnitOfMeasure,
    TreatmentApplicationMethod applicationMethod, string treatmentLocation,
    int daysUntilWithdrawalDate, DateTime? boosterDate, decimal? treatmentTotalCost,
    string description, DateTime treatmentDate, string tags) : BaseEntity
{
    public TreatmentType TreatmentType { get; private set; } = treatmentType;

    public string Product { get; private set; } = product;

    public string BatchNumber { get; private set; } = batchNumber;

    public string Dosage { get; private set; } = dosage;

    public decimal? InventoryAmountUsed { get; private set; } = inventoryAmountUsed;

    public LivestockUnitOfMeasure InventoryUnitOfMeasure { get; private set; } = inventoryUnitOfMeasure;

    public TreatmentApplicationMethod ApplicationMethod { get; private set; } = applicationMethod;

    // <summary>
    /// The body part of the livestock on which the treatment was applied.
    /// This could Rump, Flank, Leg, Head, Neck, etc.
    /// </summary>
    public string TreatmentLocation { get; private set; } = treatmentLocation;

    public int DaysUntilWithdrawalDate { get; private set; } = daysUntilWithdrawalDate;

    public DateTime? BoosterDate { get; private set; } = boosterDate;

    public decimal? TreatmentTotalCost { get; private set; } = treatmentTotalCost;

    public string Description { get; private set; } = description;

    public DateTime TreatmentDate { get; private set; } = treatmentDate;

    public string Tags { get; private set; } = tags;

    public Guid LivestockId { get; private set; }

    public Livestock Livestock { get; private set; } = default!;

    public void Update(TreatmentType treatmentType, string product, string batchNumber,
        string dosage, decimal? inventoryAmountUsed, LivestockUnitOfMeasure inventoryUnitOfMeasure,
        TreatmentApplicationMethod applicationMethod, string treatmentLocation,
        int daysUntilWithdrawalDate, DateTime? boosterDate, decimal? treatmentTotalCost,
        string description, DateTime treatmentDate, string tags)
    {
        TreatmentType = treatmentType;
        Product = product;
        BatchNumber = batchNumber;
        Dosage = dosage;
        InventoryAmountUsed = inventoryAmountUsed;
        InventoryUnitOfMeasure = inventoryUnitOfMeasure;
        ApplicationMethod = applicationMethod;
        TreatmentLocation = treatmentLocation;
        DaysUntilWithdrawalDate = daysUntilWithdrawalDate;
        BoosterDate = boosterDate;
        TreatmentTotalCost = treatmentTotalCost;
        Description = description;
        TreatmentDate = treatmentDate;
        Tags = tags;
    }

}


