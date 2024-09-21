namespace Edgias.MurimiOS.Domain.Entities.LivestockProductionAggregate;

public class LivestockMeasurement(decimal? weight, decimal? height, decimal? conditionalScore,
    decimal? temperature, decimal? fecalEggCount, DateTime measurementDate) : BaseEntity
{
    public decimal? Weight { get; private set; } = weight;

    public decimal? Height { get; private set; } = height;

    public decimal? ConditionalScore { get; private set; } = conditionalScore;

    public decimal? Temperature { get; private set; } = temperature;

    public decimal? FecalEggCount { get; private set; } = fecalEggCount;

    public DateTime MeasurementDate { get; private set; } = measurementDate;

    public Guid LivestockId { get; private set; }

    public Livestock Livestock { get; private set; } = default!;

    public void Update(decimal? weight, decimal? height, decimal? conditionalScore,
        decimal? temperature, decimal? fecalEggCount, DateTime measurementDate)
    {
        Weight = weight;
        Height = height;
        ConditionalScore = conditionalScore;
        Temperature = temperature;
        FecalEggCount = fecalEggCount;
        MeasurementDate = measurementDate;
    }
}


