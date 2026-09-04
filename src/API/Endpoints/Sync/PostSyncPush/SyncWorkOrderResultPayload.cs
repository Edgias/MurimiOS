using EdgIQ.MurimiOS.Domain.WorkOrder;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;
using WorkResultType = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkResultType;

public sealed record SyncWorkOrderResultPayload(WorkResultType ResultType, Guid ResultId, string? Notes);
