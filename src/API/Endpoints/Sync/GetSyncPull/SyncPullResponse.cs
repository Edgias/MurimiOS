using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.GetSyncPull;

public sealed record SyncPullResponse(long Checkpoint, bool HasMore, IReadOnlyList<SyncChangeResponse> Changes);
