using EdgIQ.MurimiOS.Domain.Common;




namespace EdgIQ.MurimiOS.Domain.SyncClient;

public sealed class SyncCommandLog : TenantEntity
{
    private SyncCommandLog()
    {
    }

    public SyncCommandLog(
        Guid commandId,
        Guid deviceId,
        long sequence,
        string commandType,
        int schemaVersion,
        DateTimeOffset capturedAt,
        DateTimeOffset processedAt) : base(commandId)
    {
        DeviceId = deviceId;
        Sequence = sequence;
        CommandType = commandType;
        SchemaVersion = schemaVersion;
        CapturedAt = capturedAt;
        ProcessedAt = processedAt;
    }

    public Guid DeviceId { get; private set; }

    public long Sequence { get; private set; }

    public string CommandType { get; private set; } = string.Empty;

    public int SchemaVersion { get; private set; }

    public DateTimeOffset CapturedAt { get; private set; }

    public DateTimeOffset ProcessedAt { get; private set; }

    public SyncCommandStatus Status { get; private set; }

    public Guid? ResultId { get; private set; }

    public string? ErrorCode { get; private set; }

    public string? ErrorMessage { get; private set; }

    public void MarkApplied(Guid resultId)
    {
        Status = SyncCommandStatus.Applied;
        ResultId = resultId;
    }

    public void MarkRejected(string errorCode, string errorMessage)
    {
        Status = SyncCommandStatus.Rejected;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
