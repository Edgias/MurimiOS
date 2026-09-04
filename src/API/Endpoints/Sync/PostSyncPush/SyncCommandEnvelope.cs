using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record SyncCommandEnvelope(
    Guid CommandId,
    long Sequence,
    string Type,
    int SchemaVersion,
    DateTimeOffset CapturedAt,
    JsonElement Payload);
