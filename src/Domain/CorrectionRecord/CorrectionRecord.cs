using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.CorrectionRecord;

public sealed class CorrectionRecord : TenantEntity
{
    private CorrectionRecord() { }

    public CorrectionRecord(
        Guid id,
        string entityType,
        Guid originalEntityId,
        Guid replacementEntityId,
        string reason,
        DateTimeOffset correctedAt,
        string correctedBy,
        Guid? farmId = null) : base(id)
    {
        if (originalEntityId == Guid.Empty || replacementEntityId == Guid.Empty || originalEntityId == replacementEntityId)
            throw new ArgumentException("Distinct original and replacement entity identifiers are required.");
        EntityType = RequireText(entityType, nameof(entityType), 240);
        OriginalEntityId = originalEntityId;
        ReplacementEntityId = replacementEntityId;
        Reason = RequireText(reason, nameof(reason), 2_000);
        CorrectedAt = correctedAt.ToUniversalTime();
        CorrectedBy = RequireText(correctedBy, nameof(correctedBy), 240);
        FarmId = farmId == Guid.Empty ? null : farmId;
    }

    public string EntityType { get; private set; } = string.Empty;
    public Guid OriginalEntityId { get; private set; }
    public Guid ReplacementEntityId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset CorrectedAt { get; private set; }
    public string CorrectedBy { get; private set; } = string.Empty;
    public Guid? FarmId { get; private set; }

    private static string RequireText(string? value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return result;
    }
}
