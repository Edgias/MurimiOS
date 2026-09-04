using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record AllocateLivestockFeedPayload(
    Guid SubjectId,
    Guid FeedBatchId,
    decimal Quantity,
    string Unit,
    DateTimeOffset OccurredAt,
    string? Notes);
