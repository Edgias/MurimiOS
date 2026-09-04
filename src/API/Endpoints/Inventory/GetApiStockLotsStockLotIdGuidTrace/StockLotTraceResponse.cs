using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiStockLotsStockLotIdGuidTrace;

public sealed record StockLotTraceResponse(
    Guid StockLotId,
    string StockLotCode,
    string Input,
    string Unit,
    string DisplayUnit,
    decimal QuantityOnHand,
    IReadOnlyList<InputApplicationTraceResponse> Applications,
    IReadOnlyList<HarvestTraceResponse> RelatedHarvestLots);
