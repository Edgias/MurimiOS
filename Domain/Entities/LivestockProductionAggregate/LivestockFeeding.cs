namespace Edgias.MurimiOS.Domain.Entities.LivestockProductionAggregate;

public class LivestockFeeding(decimal? amountFed, LivestockUnitOfMeasure amountFedUnitOfMeasure,
    string feedDetails, decimal? feedWeight, decimal? estimatedCost, string notes,
    DateTimeOffset feedingDate) : BaseEntity
{
    public decimal? AmountFed { get; private set; } = amountFed;

    public LivestockUnitOfMeasure AmountFedUnitOfMeasure { get; private set; } = amountFedUnitOfMeasure;

    public string FeedDetails { get; private set; } = feedDetails;

    public decimal? FeedWeight { get; private set; } = feedWeight;

    public decimal? EstimatedCost { get; private set; } = estimatedCost;

    public string Notes { get; private set; } = notes;

    public DateTimeOffset FeedingDate { get; private set; } = feedingDate;

    public Guid LivestockId { get; private set; }

    public Livestock Livestock { get; private set; } = default!;

    public void Update(decimal? amountFed, LivestockUnitOfMeasure amountFedUnitOfMeasure,
        string feedDetails, decimal? feedWeight, decimal? estimatedCost, string notes,
        DateTimeOffset feedingDate)
    {
        AmountFed = amountFed;
        AmountFedUnitOfMeasure = amountFedUnitOfMeasure;
        FeedDetails = feedDetails;
        FeedWeight = feedWeight;
        EstimatedCost = estimatedCost;
        Notes = notes;
        FeedingDate = feedingDate;
    }
}
