using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class AnalyticsForecastEvaluation : TenantEntity
{
    private AnalyticsForecastEvaluation() { }
    public AnalyticsForecastEvaluation(Guid id, Guid forecastFactId, Guid actualFactId, Guid farmId,
        string metricCode, string grainKey, decimal forecastValue, decimal actualValue, string unit,
        int horizonDays, string modelVersion, DateTimeOffset evaluatedAt) : base(id)
    {
        if (forecastFactId == Guid.Empty || actualFactId == Guid.Empty || farmId == Guid.Empty || horizonDays < 0)
            throw new ArgumentException("Forecast, actual fact, farm, and non-negative horizon are required.");
        ForecastFactId = forecastFactId; ActualFactId = actualFactId; FarmId = farmId;
        MetricCode = KpiDefinition.Text(metricCode, 120).ToLowerInvariant(); GrainKey = KpiDefinition.Text(grainKey, 300);
        ForecastValue = forecastValue; ActualValue = actualValue; Unit = KpiDefinition.Text(unit, 40);
        AbsoluteError = Math.Abs(actualValue - forecastValue); Bias = forecastValue - actualValue;
        AbsolutePercentageError = actualValue == 0 ? null : Math.Abs((actualValue - forecastValue) / actualValue) * 100m;
        HorizonDays = horizonDays; ModelVersion = KpiDefinition.Text(modelVersion, 120); EvaluatedAt = evaluatedAt.ToUniversalTime();
    }
    public Guid ForecastFactId { get; private set; }
    public Guid ActualFactId { get; private set; }
    public Guid FarmId { get; private set; }
    public string MetricCode { get; private set; } = string.Empty;
    public string GrainKey { get; private set; } = string.Empty;
    public decimal ForecastValue { get; private set; }
    public decimal ActualValue { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal AbsoluteError { get; private set; }
    public decimal? AbsolutePercentageError { get; private set; }
    public decimal Bias { get; private set; }
    public int HorizonDays { get; private set; }
    public string ModelVersion { get; private set; } = string.Empty;
    public DateTimeOffset EvaluatedAt { get; private set; }
}
