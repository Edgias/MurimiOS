using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.SupplierRiskAssessment;
using SupplierInvitation = global::EdgIQ.MurimiOS.Domain.SupplierInvitation.SupplierInvitation;

public sealed class SupplierRiskAssessment : TenantEntity
{
    private SupplierRiskAssessment() { }

    public SupplierRiskAssessment(Guid id, Guid? relationshipId, Guid? hostedSupplierId, int score,
        SupplierRiskBand band, string modelVersion, DateTimeOffset factsAsOf, string componentsJson,
        string inputsJson, DateTimeOffset assessedAt, string assessedBy) : base(id)
    {
        if (relationshipId.HasValue == hostedSupplierId.HasValue || relationshipId == Guid.Empty
            || hostedSupplierId == Guid.Empty || score is < 0 or > 100 || !Enum.IsDefined(band))
            throw new ArgumentException("A supplier target, score and band are required.");
        RelationshipId = relationshipId;
        HostedSupplierId = hostedSupplierId;
        Score = score;
        Band = band;
        ModelVersion = SupplierInvitation.Text(modelVersion, 40);
        FactsAsOf = factsAsOf.ToUniversalTime();
        ComponentsJson = JsonObject(componentsJson, nameof(componentsJson));
        InputsJson = JsonObject(inputsJson, nameof(inputsJson));
        AssessedAt = assessedAt.ToUniversalTime();
        AssessedBy = SupplierInvitation.Text(assessedBy, 240);
    }

    public Guid? RelationshipId { get; private set; }
    public Guid? HostedSupplierId { get; private set; }
    public int Score { get; private set; }
    public SupplierRiskBand Band { get; private set; }
    public string ModelVersion { get; private set; } = "";
    public DateTimeOffset FactsAsOf { get; private set; }
    public string ComponentsJson { get; private set; } = "{}";
    public string InputsJson { get; private set; } = "{}";
    public DateTimeOffset AssessedAt { get; private set; }
    public string AssessedBy { get; private set; } = "";

    private static string JsonObject(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 20_000) throw new ArgumentException("Risk JSON is empty or too large.", parameter);
        using JsonDocument document = JsonDocument.Parse(value);
        if (document.RootElement.ValueKind != JsonValueKind.Object) throw new ArgumentException("Risk JSON must be an object.", parameter);
        return document.RootElement.GetRawText();
    }
}
