using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;

public sealed record ApplyInputToCropZonePayload(
    Guid CropZoneId,
    Guid StockLotId,
    decimal Quantity,
    DateTimeOffset OccurredAt,
    string? Notes);
