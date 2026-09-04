using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record SyncPushRequest(Guid DeviceId, IReadOnlyList<SyncCommandEnvelope> Commands);
