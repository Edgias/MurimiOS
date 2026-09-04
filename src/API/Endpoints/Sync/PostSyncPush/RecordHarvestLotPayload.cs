using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record RecordHarvestLotPayload(
    Guid CropZoneId,
    string LotCode,
    decimal Quantity,
    string Unit,
    DateTimeOffset HarvestedAt,
    string? Notes);
