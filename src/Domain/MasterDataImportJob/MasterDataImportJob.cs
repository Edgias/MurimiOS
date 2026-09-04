using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.MasterDataImportJob;

public sealed class MasterDataImportJob : TenantEntity
{
    private MasterDataImportJob() { }

    public MasterDataImportJob(Guid id, MasterDataImportType type, string fileName, long byteSize,
        string sha256, string objectKey, string parserVersion, string headerJson) : base(id)
    {
        if (!Enum.IsDefined(type) || byteSize is <= 0 or > 20_971_520)
            throw new ArgumentException("Import type or source size is invalid.");
        Type = type;
        FileName = Required(fileName, 255);
        ByteSize = byteSize;
        Sha256 = Hash(sha256);
        ObjectKey = Required(objectKey, 500);
        ParserVersion = Required(parserVersion, 40);
        HeaderJson = Json(headerJson, JsonValueKind.Array, 20_000);
        Status = MasterDataImportStatus.Uploaded;
    }

    public MasterDataImportType Type { get; private set; }
    public string FileName { get; private set; } = "";
    public long ByteSize { get; private set; }
    public string Sha256 { get; private set; } = "";
    public string ObjectKey { get; private set; } = "";
    public string ParserVersion { get; private set; } = "";
    public string HeaderJson { get; private set; } = "[]";
    public MasterDataImportStatus Status { get; private set; }
    public int RowCount { get; private set; }
    public int ReadyRowCount { get; private set; }
    public int ErrorRowCount { get; private set; }
    public int AppliedRowCount { get; private set; }
    public int FailedRowCount { get; private set; }
    public int CheckpointOrdinal { get; private set; }
    public DateTimeOffset? ValidatedAt { get; private set; }
    public DateTimeOffset? ApplyStartedAt { get; private set; }
    public DateTimeOffset? AppliedAt { get; private set; }
    public string? AppliedBy { get; private set; }
    public string? FailureCode { get; private set; }
    public string? FailureDetail { get; private set; }

    public void CompleteValidation(int rowCount, int readyRows, int errorRows, DateTimeOffset at)
    {
        if (Status != MasterDataImportStatus.Uploaded || rowCount < 0 || readyRows < 0 || errorRows < 0
            || readyRows + errorRows != rowCount) throw new InvalidOperationException("Import validation counts or state are invalid.");
        RowCount = rowCount;
        ReadyRowCount = readyRows;
        ErrorRowCount = errorRows;
        ValidatedAt = at.ToUniversalTime();
        Status = readyRows > 0 ? MasterDataImportStatus.Validated : MasterDataImportStatus.Rejected;
    }

    public void BeginApply(string actor, DateTimeOffset at)
    {
        if (Status == MasterDataImportStatus.Applied) return;
        if (Status is not (MasterDataImportStatus.Validated or MasterDataImportStatus.Applying))
            throw new InvalidOperationException("Only a validated import can be applied.");
        if (Status == MasterDataImportStatus.Validated)
        {
            Status = MasterDataImportStatus.Applying;
            ApplyStartedAt = at.ToUniversalTime();
            AppliedBy = Required(actor, 240);
        }
    }

    public void RecordBatch(int checkpointOrdinal, int appliedRows, int failedRows, bool completed, DateTimeOffset at)
    {
        if (Status != MasterDataImportStatus.Applying || checkpointOrdinal < CheckpointOrdinal
            || appliedRows < AppliedRowCount || failedRows < FailedRowCount)
            throw new InvalidOperationException("Import checkpoint cannot move backwards.");
        CheckpointOrdinal = checkpointOrdinal;
        AppliedRowCount = appliedRows;
        FailedRowCount = failedRows;
        if (completed)
        {
            Status = MasterDataImportStatus.Applied;
            AppliedAt = at.ToUniversalTime();
        }
    }

    public void Fail(string code, string detail, DateTimeOffset at)
    {
        if (Status == MasterDataImportStatus.Applied) throw new InvalidOperationException("An applied import cannot fail.");
        FailureCode = Required(code, 80).ToLowerInvariant();
        FailureDetail = Required(detail, 2_000);
        ValidatedAt ??= at.ToUniversalTime();
        Status = MasterDataImportStatus.Failed;
    }

    private static string Required(string? value, int maximum)
    {
        string result = value?.Trim() ?? "";
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"Text must contain 1-{maximum} characters.");
        return result;
    }

    private static string Hash(string? value)
    {
        string result = value?.Trim().ToLowerInvariant() ?? "";
        if (result.Length != 64 || result.Any(x => !Uri.IsHexDigit(x))) throw new ArgumentException("A SHA-256 digest is required.");
        return result;
    }

    internal static string Json(string value, JsonValueKind kind, int maximum)
    {
        if (value.Length > maximum) throw new ArgumentException("Import JSON payload is too large.");
        using JsonDocument document = JsonDocument.Parse(value);
        if (document.RootElement.ValueKind != kind) throw new ArgumentException($"Import JSON must be a {kind}.");
        return document.RootElement.GetRawText();
    }
}
