using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SoilSample;

public sealed class SoilSampleResult : TenantEntity
{
    private SoilSampleResult() { }
    public SoilSampleResult(Guid id, Guid sampleId, string analyteCode, string analyteName, decimal value,
        string unit, string? method, decimal? detectionLimit, string? resultFlag, DateTimeOffset recordedAt,
        string recordedBy) : base(id)
    {
        if (sampleId == Guid.Empty || detectionLimit < 0) throw new ArgumentException("Soil sample and detection limit are invalid.");
        SampleId = sampleId; AnalyteCode = DigitalTwinText.Required(analyteCode, 80).ToUpperInvariant();
        AnalyteName = DigitalTwinText.Required(analyteName, 160); Value = value; Unit = DigitalTwinText.Required(unit, 40);
        Method = DigitalTwinText.Optional(method, 160); DetectionLimit = detectionLimit;
        ResultFlag = DigitalTwinText.Optional(resultFlag, 40); RecordedAt = recordedAt.ToUniversalTime();
        RecordedBy = DigitalTwinText.Required(recordedBy, 240);
    }
    public Guid SampleId { get; private set; }
    public string AnalyteCode { get; private set; } = "";
    public string AnalyteName { get; private set; } = "";
    public decimal Value { get; private set; }
    public string Unit { get; private set; } = "";
    public string? Method { get; private set; }
    public decimal? DetectionLimit { get; private set; }
    public string? ResultFlag { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public string RecordedBy { get; private set; } = "";
}
