using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record RecordCropZoneOperationPayload(
    Guid CropZoneId,
    string OperationType,
    DateTimeOffset OccurredAt,
    string? Notes);
