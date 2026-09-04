using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record SyncPushResponse(long ExpectedNextSequence, IReadOnlyList<SyncCommandResult> Results);
