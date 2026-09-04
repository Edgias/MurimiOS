using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record StartWorkOrderPayload(Guid WorkOrderId, DateTimeOffset StartedAt);
