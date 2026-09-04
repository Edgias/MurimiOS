using EdgIQ.MurimiOS.Domain.CropZone;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiStockLotsStockLotIdGuidTrace;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;

public sealed record InputApplicationTraceResponse(
    Guid MovementId,
    Guid OperationId,
    Guid CropZoneId,
    string CropZone,
    string OperationType,
    decimal Quantity,
    DateTimeOffset OccurredAt);
