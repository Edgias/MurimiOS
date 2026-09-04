using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record RecordLivestockMeasurementPayload(
    Guid SubjectId,
    string Metric,
    decimal Value,
    string Unit,
    DateTimeOffset OccurredAt);
