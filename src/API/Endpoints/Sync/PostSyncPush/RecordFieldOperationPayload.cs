using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record RecordFieldOperationPayload(
    Guid FieldId,
    Guid BoundaryVersionId,
    string OperationType,
    DateTimeOffset OccurredAt,
    string? Notes);
