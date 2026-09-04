namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed class BenchmarkAggregateProjection
{
    public int CohortSize { get; init; }
    public decimal? Minimum { get; init; }
    public decimal? Percentile25 { get; init; }
    public decimal? Median { get; init; }
    public decimal? Percentile75 { get; init; }
    public decimal? Maximum { get; init; }
    public decimal? Average { get; init; }
    public decimal? AverageDataQualityPercent { get; init; }
}
