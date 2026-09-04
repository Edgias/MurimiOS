using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record SyncGapResponse(long ExpectedSequence, Guid? CommandId, long? ReceivedSequence);
