using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiStockLotsStockLotIdGuidTrace;

public sealed record HarvestTraceResponse(
    Guid HarvestLotId,
    Guid CropZoneId,
    string LotCode,
    decimal Quantity,
    string Unit,
    decimal EnteredQuantity,
    string EnteredUnit,
    string DisplayUnit,
    DateTimeOffset HarvestedAt);
