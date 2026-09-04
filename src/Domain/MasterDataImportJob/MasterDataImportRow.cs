using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MasterDataImportJob;

public sealed class MasterDataImportRow : TenantEntity
{
    private MasterDataImportRow() { }

    public MasterDataImportRow(Guid id, Guid jobId, int ordinal, string rawJson, string normalizedJson,
        string issuesJson, bool ready) : base(id)
    {
        if (jobId == Guid.Empty || ordinal < 1) throw new ArgumentException("Import job and positive row ordinal are required.");
        JobId = jobId;
        Ordinal = ordinal;
        RawJson = MasterDataImportJob.Json(rawJson, JsonValueKind.Object, 100_000);
        NormalizedJson = MasterDataImportJob.Json(normalizedJson, JsonValueKind.Object, 100_000);
        IssuesJson = MasterDataImportJob.Json(issuesJson, JsonValueKind.Array, 100_000);
        Status = ready ? MasterDataImportRowStatus.Ready : MasterDataImportRowStatus.Invalid;
    }

    public Guid JobId { get; private set; }
    public int Ordinal { get; private set; }
    public string RawJson { get; private set; } = "{}";
    public string NormalizedJson { get; private set; } = "{}";
    public string IssuesJson { get; private set; } = "[]";
    public MasterDataImportRowStatus Status { get; private set; }
    public string? AppliedEntityType { get; private set; }
    public Guid? AppliedEntityId { get; private set; }
    public Guid? AppliedFarmId { get; private set; }
    public DateTimeOffset? AppliedAt { get; private set; }
    public string? FailureCode { get; private set; }
    public string? FailureDetail { get; private set; }

    public void MarkApplied(string entityType, Guid entityId, Guid? farmId, DateTimeOffset at)
    {
        if (Status == MasterDataImportRowStatus.Applied)
        {
            if (AppliedEntityType == entityType && AppliedEntityId == entityId) return;
            throw new InvalidOperationException("An import row cannot target two entities.");
        }
        if (Status != MasterDataImportRowStatus.Ready || entityId == Guid.Empty || farmId == Guid.Empty)
            throw new InvalidOperationException("Only a ready import row can be applied.");
        AppliedEntityType = entityType.Trim();
        AppliedEntityId = entityId;
        AppliedFarmId = farmId;
        AppliedAt = at.ToUniversalTime();
        Status = MasterDataImportRowStatus.Applied;
    }

    public void MarkFailed(string code, string detail)
    {
        if (Status != MasterDataImportRowStatus.Ready) throw new InvalidOperationException("Only a ready row can fail application.");
        FailureCode = code.Trim().ToLowerInvariant();
        FailureDetail = detail.Trim();
        Status = MasterDataImportRowStatus.Failed;
    }
}
