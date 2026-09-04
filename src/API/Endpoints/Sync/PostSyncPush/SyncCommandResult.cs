using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record SyncCommandResult(
    Guid CommandId,
    long Sequence,
    string Status,
    Guid? ResultId,
    string? ErrorCode,
    string? ErrorMessage);
