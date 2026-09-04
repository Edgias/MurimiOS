using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.ComplianceAuditPackExport;
using ComplianceStandardVersion = global::EdgIQ.MurimiOS.Domain.ComplianceStandardVersion.ComplianceStandardVersion;

public sealed class ComplianceAuditPackExport : TenantEntity
{
    private ComplianceAuditPackExport() { }

    public ComplianceAuditPackExport(Guid id, Guid assessmentId, Guid farmId, Guid standardVersionId,
        int revision, int contentSchemaVersion, string scopeStatement, bool includesEvidenceFiles,
        ComplianceAuditPackFormat format, string objectKey, long byteSize, int contentEntryCount,
        int evidenceFileCount, string artifactSha256, string manifestSha256, string signatureAlgorithm,
        string signatureKeyId, string signatureBase64, string manifestJson, DateTimeOffset generatedAt,
        string generatedBy) : base(id)
    {
        if (assessmentId == Guid.Empty || farmId == Guid.Empty || standardVersionId == Guid.Empty)
            throw new ArgumentException("Assessment, farm, and standard identifiers are required.");
        if (revision <= 0 || contentSchemaVersion <= 0 || byteSize <= 0 || contentEntryCount <= 0
            || evidenceFileCount < 0 || evidenceFileCount > contentEntryCount || !Enum.IsDefined(format))
            throw new ArgumentOutOfRangeException(nameof(revision), "Audit-pack version or content totals are invalid.");
        AssessmentId = assessmentId; FarmId = farmId; StandardVersionId = standardVersionId;
        Revision = revision; ContentSchemaVersion = contentSchemaVersion;
        ScopeStatement = ComplianceStandardVersion.Text(scopeStatement, 4_000);
        IncludesEvidenceFiles = includesEvidenceFiles; Format = format;
        ObjectKey = ComplianceStandardVersion.Text(objectKey, 1_000); ByteSize = byteSize;
        ContentEntryCount = contentEntryCount; EvidenceFileCount = evidenceFileCount;
        ArtifactSha256 = Sha(artifactSha256); ManifestSha256 = Sha(manifestSha256);
        SignatureAlgorithm = ComplianceStandardVersion.Text(signatureAlgorithm, 80);
        SignatureKeyId = ComplianceStandardVersion.Text(signatureKeyId, 160);
        SignatureBase64 = Base64(signatureBase64, 2_000);
        ManifestJson = JsonObject(manifestJson, 1_000_000);
        GeneratedAt = generatedAt.ToUniversalTime(); GeneratedBy = ComplianceStandardVersion.Text(generatedBy, 240);
    }

    public Guid AssessmentId { get; private set; }
    public Guid FarmId { get; private set; }
    public Guid StandardVersionId { get; private set; }
    public int Revision { get; private set; }
    public int ContentSchemaVersion { get; private set; }
    public string ScopeStatement { get; private set; } = "";
    public bool IncludesEvidenceFiles { get; private set; }
    public ComplianceAuditPackFormat Format { get; private set; }
    public string ObjectKey { get; private set; } = "";
    public long ByteSize { get; private set; }
    public int ContentEntryCount { get; private set; }
    public int EvidenceFileCount { get; private set; }
    public string ArtifactSha256 { get; private set; } = "";
    public string ManifestSha256 { get; private set; } = "";
    public string SignatureAlgorithm { get; private set; } = "";
    public string SignatureKeyId { get; private set; } = "";
    public string SignatureBase64 { get; private set; } = "";
    public string ManifestJson { get; private set; } = "{}";
    public DateTimeOffset GeneratedAt { get; private set; }
    public string GeneratedBy { get; private set; } = "";

    private static string Sha(string? value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? "";
        if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x)))
            throw new ArgumentException("A hexadecimal SHA-256 digest is required.");
        return result;
    }

    private static string Base64(string? value, int maximum)
    {
        string result = ComplianceStandardVersion.Text(value, maximum);
        try { Convert.FromBase64String(result); }
        catch (FormatException exception) { throw new ArgumentException("A base64 signature is required.", exception); }
        return result;
    }

    private static string JsonObject(string? value, int maximum)
    {
        string result = ComplianceStandardVersion.Text(value, maximum);
        try
        {
            using System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(result);
            if (document.RootElement.ValueKind != System.Text.Json.JsonValueKind.Object)
                throw new ArgumentException("The audit-pack manifest must be a JSON object.");
            return document.RootElement.GetRawText();
        }
        catch (System.Text.Json.JsonException exception)
        { throw new ArgumentException("The audit-pack manifest must be valid JSON.", exception); }
    }
}
