using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.GetSyncPull;

public sealed record SyncChangeResponse(
    long Sequence,
    string EntityType,
    Guid EntityId,
    string Operation,
    JsonElement Payload,
    DateTimeOffset ChangedAt);
