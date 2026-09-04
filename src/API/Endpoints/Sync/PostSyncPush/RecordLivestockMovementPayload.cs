using EdgIQ.MurimiOS.Domain.LivestockSubject;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;
using LivestockMovementType = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementType;

public sealed record RecordLivestockMovementPayload(
    Guid SubjectId,
    LivestockMovementType Type,
    Guid? FromHoldingId,
    Guid? ToHoldingId,
    DateTimeOffset OccurredAt,
    string? Reference,
    string? Notes);
