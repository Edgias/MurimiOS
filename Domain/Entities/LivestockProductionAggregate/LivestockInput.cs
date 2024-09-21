namespace Edgias.MurimiOS.Domain.Entities.LivestockProductionAggregate;

public class LivestockInput(decimal? amount, LivestockUnitOfMeasure amountUnitOfMeasure,
    string inputCategory, decimal? estimatedCost, string notes,
    DateTime inputDate) : BaseEntity
{
    public decimal? Amount { get; private set; } = amount;

    public LivestockUnitOfMeasure AmountUnitOfMeasure { get; private set; } = amountUnitOfMeasure;

    public string InputCategory { get; private set; } = inputCategory;

    public decimal? EstimatedCost { get; private set; } = estimatedCost;

    public string Notes { get; private set; } = notes;

    public DateTime InputDate { get; private set; } = inputDate;

    public Guid LivestockId { get; private set; }

    public Livestock Livestock { get; private set; } = default!;

    public void Update(decimal? amount, LivestockUnitOfMeasure amountUnitOfMeasure,
        string inputCategory, decimal? estimatedCost, string notes,
        DateTime inputDate)
    {
        Amount = amount;
        AmountUnitOfMeasure = amountUnitOfMeasure;
        InputCategory = inputCategory;
        EstimatedCost = estimatedCost;
        Notes = notes;
        InputDate = inputDate;
    }
}


