using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class AnalyticsDimensionSnapshot : TenantEntity
{
    private AnalyticsDimensionSnapshot() { }
    public AnalyticsDimensionSnapshot(Guid id, Guid projectionRunId, Guid farmId, AnalyticsDimensionType dimensionType,
        Guid entityId, string grainKey, string naturalKey, string attributesJson, DateOnly validOn) : base(id)
    {
        if (projectionRunId == Guid.Empty || farmId == Guid.Empty || entityId == Guid.Empty || !Enum.IsDefined(dimensionType))
            throw new ArgumentException("Projection, farm, entity and dimension type are required.");
        ProjectionRunId = projectionRunId; FarmId = farmId; DimensionType = dimensionType; EntityId = entityId;
        GrainKey = KpiDefinition.Text(grainKey, 300); NaturalKey = KpiDefinition.Text(naturalKey, 300);
        AttributesJson = Json(attributesJson); AttributesSha256 = Sha(AttributesJson); ValidOn = validOn;
    }
    public Guid ProjectionRunId { get; private set; }
    public Guid FarmId { get; private set; }
    public AnalyticsDimensionType DimensionType { get; private set; }
    public Guid EntityId { get; private set; }
    public string GrainKey { get; private set; } = string.Empty;
    public string NaturalKey { get; private set; } = string.Empty;
    public string AttributesJson { get; private set; } = "{}";
    public string AttributesSha256 { get; private set; } = string.Empty;
    public DateOnly ValidOn { get; private set; }
    private static string Json(string value) { string result = KpiDefinition.Text(value, 16_000); using JsonDocument _ = JsonDocument.Parse(result); return result; }
    private static string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
