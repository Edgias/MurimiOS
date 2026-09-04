using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record CompleteWorkOrderPayload(
    Guid WorkOrderId,
    DateTimeOffset CompletedAt,
    string? Notes,
    IReadOnlyList<SyncWorkOrderResultPayload>? Results);
